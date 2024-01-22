using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using EntityTagHeaderValue = Microsoft.Net.Http.Headers.EntityTagHeaderValue;

namespace MinimalApiBuilder.Middleware;

/// <summary>
/// Middleware for serving static files, choosing pre-compressed files based on the Accept-Encoding header.
/// </summary>
public class CompressedStaticFileMiddleware : IMiddleware
{
    private static readonly StringValues s_acceptRanges = new("bytes");

    private readonly CompressedStaticFileOptions _options;
    private readonly ILogger<CompressedStaticFileMiddleware> _logger;
    private readonly IFileProvider _fileProvider;
    private readonly IContentTypeProvider _contentTypeProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompressedStaticFileMiddleware" /> class.
    /// </summary>
    /// <param name="options">The middleware's static file options.</param>
    /// <param name="logger">The middleware's logger.</param>
    public CompressedStaticFileMiddleware(IOptions<CompressedStaticFileOptions> options,
        ILogger<CompressedStaticFileMiddleware> logger)
    {
        _options = options.Value;
        _logger = logger;
        _fileProvider = _options.FileProvider!;
        _contentTypeProvider = _options.ContentTypeProvider;
    }

    /// <inheritdoc />
    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (HasEndpointDelegate(context))
        {
            _logger.EndpointMatched();
            return next(context);
        }

        bool isGet = HttpMethods.IsGet(context.Request.Method);
        bool isHead = HttpMethods.IsHead(context.Request.Method);

        if (!isGet && !isHead)
        {
            _logger.InvalidMethod(context.Request.Method);
            return next(context);
        }

        if (!context.Request.Path.StartsWithSegments(_options.RequestPath, out PathString remaining))
        {
            _logger.PathMismatch(_options.RequestPath);
            return next(context);
        }

        string subPath = remaining;

        if (!TryGetContentType(subPath, out string? contentType))
        {
            _logger.InvalidContentType();
            return next(context);
        }

        RequestHeaders requestHeaders = context.Request.GetTypedHeaders();
        ResponseHeaders responseHeaders = context.Response.GetTypedHeaders();

        IFileInfo fileInfo = _fileProvider.GetFileInfo(subPath);
        StringSegment filename = fileInfo.Name;

        if (AcceptEncodingHelper.TryGetContentCoding(requestHeaders, _options, fileInfo.Exists,
            out string? contentCoding, out string? extension, out IdentityAllowedFlags identity))
        {
            IFileInfo compressedFileInfo = _fileProvider.GetFileInfo($"{subPath}.{extension}");

            if (compressedFileInfo.Exists)
            {
                fileInfo = compressedFileInfo;
                filename = new StringSegment(compressedFileInfo.Name, 0,
                    compressedFileInfo.Name.Length - extension.Length - 1);
            }
            else if (identity.IsNotAllowed())
            {
                return ContentCodingContentNegotiationFailed(context);
            }
            else
            {
                contentCoding = null;
            }
        }
        else if (identity.IsNotAllowed())
        {
            return ContentCodingContentNegotiationFailed(context);
        }

        if (!fileInfo.Exists)
        {
            _logger.FileDoesNotExist();
            return next(context);
        }

        CompressedStaticFileResponseContext responseContext = new(context, filename, contentCoding);
        (EntityTagHeaderValue etag, DateTimeOffset lastModified) = GetEtagAndLastModified(fileInfo);

        if (isHead)
        {
            responseHeaders.LastModified = lastModified;
            responseHeaders.ETag = etag;
            responseHeaders.Headers.AcceptRanges = s_acceptRanges;
            responseHeaders.Headers.ContentEncoding = contentCoding;
            context.Response.ContentType = contentType;
            context.Response.ContentLength = fileInfo.Length;
            _options.OnPrepareResponse(responseContext);
            return _options.OnPrepareResponseAsync(responseContext);
        }

        PreconditionState precondition = PreconditionHelper.EvaluatePreconditions(requestHeaders, etag, lastModified);

        switch (precondition)
        {
            case PreconditionState.ShouldProcess:
                responseHeaders.LastModified = lastModified;
                responseHeaders.ETag = etag;
                responseHeaders.Headers.AcceptRanges = s_acceptRanges;
                responseHeaders.Headers.ContentEncoding = contentCoding;
                context.Response.ContentType = contentType;

                // https://www.rfc-editor.org/rfc/rfc9110.html#section-13.1.5-6
                // A server MUST ignore an If-Range header field received in a request that does not contain a Range header field.
                if (RangeHelper.HasRangeHeaderField(context) &&
                    PreconditionHelper.EvaluateIfRange(requestHeaders, etag, lastModified) == (true, false))
                {
                    // https://www.rfc-editor.org/rfc/rfc9110.html#section-13.2.2-3.5.1
                    // Respond with the entire representation when the If-Range precondition is false.
                    _logger.IfRangePreconditionFailed(subPath);
                    return SendFileAsync(responseContext, fileInfo, next, subPath);
                }

                if (!RangeHelper.TryParseRange(context, requestHeaders, fileInfo.Length, out (long, long)? range))
                {
                    return SendFileAsync(responseContext, fileInfo, next, subPath);
                }

                // https://www.rfc-editor.org/rfc/rfc9110.html#section-14.2-13
                // If all of the preconditions are true, the server supports the Range header field for the target resource,
                // the received Range field-value contains a valid ranges-specifier, and either the range-unit is not supported
                // for that target resource or the ranges-specifier is unsatisfiable with respect to the selected representation,
                // the server SHOULD send a 416 (Range Not Satisfiable) response.
                if (range is null)
                {
                    SetStatusCode(context, StatusCodes.Status416RangeNotSatisfiable);
                    // https://www.rfc-editor.org/rfc/rfc9110.html#section-15.5.17-3
                    // A server that generates a 416 response to a byte-range request SHOULD generate a Content-Range header field
                    // specifying the current length of the selected representation (Section 14.4).
                    responseHeaders.ContentRange = new ContentRangeHeaderValue(fileInfo.Length);
                    _logger.RangeNotSatisfiable(context.Request.Headers.Range, subPath);
                    return Task.CompletedTask;
                }

                (long start, long end) = range.Value;
                long count = end - start + 1;

                responseHeaders.ContentRange = new ContentRangeHeaderValue(start, end, fileInfo.Length);
                SetStatusCode(context, StatusCodes.Status206PartialContent);
                context.Response.ContentLength = count;
                _logger.SendingRange(start, end, subPath);
                return SendFileAsync(responseContext, fileInfo, next, start, count);
            case PreconditionState.NotModified:
                responseHeaders.LastModified = lastModified;
                responseHeaders.ETag = etag;
                responseHeaders.Headers.AcceptRanges = s_acceptRanges;
                responseHeaders.Headers.ContentEncoding = contentCoding;
                SetStatusCode(context, StatusCodes.Status304NotModified);
                context.Response.ContentType = contentType;
                context.Response.ContentLength = fileInfo.Length;
                _logger.NotModified(subPath);
                return Task.CompletedTask;
            case PreconditionState.PreconditionFailed:
                SetStatusCode(context, StatusCodes.Status412PreconditionFailed);
                _logger.PreconditionFailed(subPath);
                return Task.CompletedTask;
            default:
                InvalidOperationException exception = new($"Unexpected precondition state value: {precondition}");
                Debug.Fail(exception.ToString());
                throw exception;
        }
    }

    private async Task SendFileAsync(CompressedStaticFileResponseContext context,
        IFileInfo fileInfo, RequestDelegate next, long offset, long count)
    {
        SetCompressionMode(context.Context);

        _options.OnPrepareResponse(context);
        await _options.OnPrepareResponseAsync(context);

        try
        {
            await context.Context.Response.SendFileAsync(fileInfo, offset, count, context.Context.RequestAborted);
        }
        catch (OperationCanceledException e)
        {
            _logger.SendFileCancelled(e);
        }
        catch (FileNotFoundException)
        {
            context.Context.Response.Clear();
            await next(context.Context);
        }
    }

    private Task SendFileAsync(CompressedStaticFileResponseContext context,
        IFileInfo fileInfo, RequestDelegate next, string subPath)
    {
        context.Context.Response.ContentLength = fileInfo.Length;
        _logger.SendingFile(subPath);
        return SendFileAsync(context, fileInfo, next, 0, fileInfo.Length);
    }

    // https://www.rfc-editor.org/rfc/rfc9110.html#section-12.5.3-15
    // Servers that fail a request due to an unsupported content coding ought to respond with a
    // 415 (Unsupported Media Type) status and include an Accept-Encoding header field in that response,
    // allowing clients to distinguish between issues related to content codings and media types.
    private Task ContentCodingContentNegotiationFailed(HttpContext context)
    {
        SetStatusCode(context, StatusCodes.Status415UnsupportedMediaType);
        context.Response.Headers.AcceptEncoding = _options.AcceptEncoding;
        _logger.ContentCodingContentNegotiationFailed(context.Request.Headers.AcceptEncoding);
        return Task.CompletedTask;
    }

    private bool TryGetContentType(string subPath, out string? contentType)
    {
        if (_contentTypeProvider.TryGetContentType(subPath, out contentType))
        {
            return true;
        }

        contentType = _options.DefaultContentType;

        return _options.ServeUnknownFileTypes;
    }

    private static bool HasEndpointDelegate(HttpContext context)
    {
        return context.GetEndpoint()?.RequestDelegate is not null;
    }

    private static (EntityTagHeaderValue, DateTimeOffset) GetEtagAndLastModified(IFileInfo fileInfo)
    {
        DateTimeOffset last = fileInfo.LastModified;
        // Truncate to second precision
        DateTimeOffset lastModified = new DateTimeOffset(last.Year, last.Month, last.Day,
            last.Hour, last.Minute, last.Second, last.Offset).ToUniversalTime();

        string etagValue = string.Create(18, lastModified.ToFileTime() ^ fileInfo.Length, static (span, hash) =>
        {
            if (Vector256.IsHardwareAccelerated)
            {
                Vector256<short> values = Vector256.Create(
                    (short)(hash & 15),
                    (short)((hash >> 4) & 15),
                    (short)((hash >> 8) & 15),
                    (short)((hash >> 12) & 15),
                    (short)((hash >> 16) & 15),
                    (short)((hash >> 20) & 15),
                    (short)((hash >> 24) & 15),
                    (short)((hash >> 28) & 15),
                    (short)((hash >> 32) & 15),
                    (short)((hash >> 36) & 15),
                    (short)((hash >> 40) & 15),
                    (short)((hash >> 44) & 15),
                    (short)((hash >> 48) & 15),
                    (short)((hash >> 52) & 15),
                    (short)((hash >> 56) & 15),
                    (short)((hash >> 60) & 15));

                Vector256<short> condition = Vector256.LessThan(values, Vector256.Create<short>(10));
                Vector256<short> blend = Vector256.ConditionalSelect(condition,
                    Vector256.Create<short>(48), // 0-9 when less than 10
                    Vector256.Create<short>(87)); // a-f else
                Vector256<short> result = Vector256.Add(values, blend);

                span[0] = '"';
                result.StoreUnsafe(ref Unsafe.As<char, short>(ref span[1]));
                span[17] = '"';
            }
            else
            {
                byte a = (byte)(hash & 15);
                byte b = (byte)((hash >> 4) & 15);
                byte c = (byte)((hash >> 8) & 15);
                byte d = (byte)((hash >> 12) & 15);
                byte e = (byte)((hash >> 16) & 15);
                byte f = (byte)((hash >> 20) & 15);
                byte g = (byte)((hash >> 24) & 15);
                byte h = (byte)((hash >> 28) & 15);
                byte i = (byte)((hash >> 32) & 15);
                byte j = (byte)((hash >> 36) & 15);
                byte k = (byte)((hash >> 40) & 15);
                byte l = (byte)((hash >> 44) & 15);
                byte m = (byte)((hash >> 48) & 15);
                byte n = (byte)((hash >> 52) & 15);
                byte o = (byte)((hash >> 56) & 15);
                byte p = (byte)((hash >> 60) & 15);

                span[0] = '"';
                span[1] = (char)(a + (a < 10 ? 48 : 87));
                span[2] = (char)(b + (b < 10 ? 48 : 87));
                span[3] = (char)(c + (c < 10 ? 48 : 87));
                span[4] = (char)(d + (d < 10 ? 48 : 87));
                span[5] = (char)(e + (e < 10 ? 48 : 87));
                span[6] = (char)(f + (f < 10 ? 48 : 87));
                span[7] = (char)(g + (g < 10 ? 48 : 87));
                span[8] = (char)(h + (h < 10 ? 48 : 87));
                span[9] = (char)(i + (i < 10 ? 48 : 87));
                span[10] = (char)(j + (j < 10 ? 48 : 87));
                span[11] = (char)(k + (k < 10 ? 48 : 87));
                span[12] = (char)(l + (l < 10 ? 48 : 87));
                span[13] = (char)(m + (m < 10 ? 48 : 87));
                span[14] = (char)(n + (n < 10 ? 48 : 87));
                span[15] = (char)(o + (o < 10 ? 48 : 87));
                span[16] = (char)(p + (p < 10 ? 48 : 87));
                span[17] = '"';
            }
        });

        return (new EntityTagHeaderValue(etagValue), lastModified);
    }

    private void SetCompressionMode(HttpContext context)
    {
        IHttpsCompressionFeature? responseCompressionFeature = context.Features.Get<IHttpsCompressionFeature>();
        if (responseCompressionFeature is not null)
        {
            responseCompressionFeature.Mode = _options.HttpsCompression;
        }
    }

    private static void SetStatusCode(HttpContext context, int statusCode)
    {
        context.Response.StatusCode = context.Response.StatusCode == StatusCodes.Status200OK
            ? statusCode
            : context.Response.StatusCode;
    }
}
