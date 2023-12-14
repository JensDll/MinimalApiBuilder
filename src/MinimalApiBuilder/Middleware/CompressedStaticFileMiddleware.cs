using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
using StringWithQualityHeaderValue = Microsoft.Net.Http.Headers.StringWithQualityHeaderValue;

namespace MinimalApiBuilder.Middleware;

/// <summary>
/// Middleware for serving static files, choosing pre-compressed files based on the Accept-Encoding header.
/// </summary>
public class CompressedStaticFileMiddleware : IMiddleware
{
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
        PathString requestPath = context.Request.Path;

        using IDisposable? scope = _logger.CompressedStaticFileMiddleware(requestPath);

        if (HasEndpointDelegate(context))
        {
            _logger.EndpointMatched();
            return next(context);
        }

        if (!IsValidMethod(context))
        {
            _logger.InvalidMethod(context.Request.Method);
            return next(context);
        }

        if (!requestPath.StartsWithSegments(_options.RequestPath, out PathString remaining))
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

        IFileInfo fileInfo = GetFileInfo(requestHeaders, subPath, out StringSegment contentEncoding);

        if (!fileInfo.Exists)
        {
            _logger.FileDoesNotExist();
            return next(context);
        }

        StaticFileResponseContext staticFileResponseContext = new(context, fileInfo);
        (EntityTagHeaderValue etag, DateTimeOffset lastModified) = GetEtagAndLastModified(fileInfo);

        if (HttpMethods.IsHead(context.Request.Method))
        {
            responseHeaders.LastModified = lastModified;
            responseHeaders.ETag = etag;
            responseHeaders.Headers.AcceptRanges = "bytes";
            SetContentEncoding(responseHeaders, contentEncoding);
            context.Response.ContentType = contentType;
            context.Response.ContentLength = fileInfo.Length;
            _options.OnPrepareResponse(staticFileResponseContext);
#if NET8_0_OR_GREATER
            return _options.OnPrepareResponseAsync(staticFileResponseContext);
#else
            return Task.CompletedTask;
#endif
        }

        PreconditionState precondition = PreconditionHelper.EvaluatePreconditions(requestHeaders, etag, lastModified);

        switch (precondition)
        {
            case PreconditionState.ShouldProcess:
                responseHeaders.LastModified = lastModified;
                responseHeaders.ETag = etag;
                responseHeaders.Headers.AcceptRanges = "bytes";
                SetContentEncoding(responseHeaders, contentEncoding);
                context.Response.ContentType = contentType;

                if (RangeHelper.HasRangeHeaderField(context) &&
                    PreconditionHelper.EvaluateIfRange(requestHeaders, etag, lastModified) == (true, false))
                {
                    _logger.IfRangePreconditionFailed(subPath);
                    return SendFileAsync(staticFileResponseContext, next, subPath);
                }

                if (!RangeHelper.TryParseRange(context, requestHeaders, fileInfo.Length, out (long, long)? range))
                {
                    return SendFileAsync(staticFileResponseContext, next, subPath);
                }

                if (range is null)
                {
                    SetStatusCode(context, StatusCodes.Status416RangeNotSatisfiable);
                    responseHeaders.ContentRange = new ContentRangeHeaderValue(fileInfo.Length);
                    _logger.RangeNotSatisfiable(context.Request.Headers.Range, subPath);
#if NET8_0_OR_GREATER
                    return _options.OnPrepareResponseAsync(staticFileResponseContext);
#else
                    return Task.CompletedTask;
#endif
                }

                (long start, long end) = range.Value;
                long count = end - start + 1;

                responseHeaders.ContentRange = new ContentRangeHeaderValue(start, end, fileInfo.Length);
                SetStatusCode(context, StatusCodes.Status206PartialContent);
                context.Response.ContentLength = count;
                _logger.SendingRange(start, end, subPath);
                return SendFileAsync(staticFileResponseContext, next, start, count);
            case PreconditionState.NotModified:
                responseHeaders.LastModified = lastModified;
                responseHeaders.ETag = etag;
                responseHeaders.Headers.AcceptRanges = "bytes";
                SetContentEncoding(responseHeaders, contentEncoding);
                SetStatusCode(context, StatusCodes.Status304NotModified);
                context.Response.ContentType = contentType;
                context.Response.ContentLength = fileInfo.Length;
                _logger.NotModified(subPath);
                _options.OnPrepareResponse(staticFileResponseContext);
#if NET8_0_OR_GREATER
                return _options.OnPrepareResponseAsync(staticFileResponseContext);
#else
                return Task.CompletedTask;
#endif
            case PreconditionState.PreconditionFailed:
                SetStatusCode(context, StatusCodes.Status412PreconditionFailed);
                _logger.PreconditionFailed(subPath);
                _options.OnPrepareResponse(staticFileResponseContext);
#if NET8_0_OR_GREATER
                return _options.OnPrepareResponseAsync(staticFileResponseContext);
#else
                return Task.CompletedTask;
#endif
            default:
                InvalidOperationException exception = new($"Unexpected precondition state value: {precondition}");
                Debug.Fail(exception.ToString());
                throw exception;
        }
    }

    private IFileInfo GetFileInfo(RequestHeaders requestHeaders, string subPath, out StringSegment contentEncoding)
    {
        IFileInfo fileInfo = _fileProvider.GetFileInfo(subPath);

        if (!TryGetContentEncoding(requestHeaders, out contentEncoding, out string? extension))
        {
            return fileInfo;
        }

        IFileInfo compressedFileInfo = _fileProvider.GetFileInfo($"{subPath}.{extension}");

        if (compressedFileInfo.Exists)
        {
            return compressedFileInfo;
        }

        contentEncoding = null;

        return fileInfo;
    }

    private async Task SendFileAsync(StaticFileResponseContext context, RequestDelegate next, long offset, long count)
    {
        SetCompressionMode(context.Context);

        _options.OnPrepareResponse(context);
#if NET8_0_OR_GREATER
        await _options.OnPrepareResponseAsync(context);
#endif

        try
        {
            await context.Context.Response.SendFileAsync(context.File, offset, count, context.Context.RequestAborted);
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

    private Task SendFileAsync(StaticFileResponseContext context, RequestDelegate next, string subPath)
    {
        context.Context.Response.ContentLength = context.File.Length;
        _logger.SendingFile(subPath);
        return SendFileAsync(context, next, 0, context.File.Length);
    }

    // https://www.rfc-editor.org/rfc/rfc9110.html#section-12.5.3-9
    private bool TryGetContentEncoding(RequestHeaders requestHeaders,
        out StringSegment contentEncoding,
        [NotNullWhen(true)] out string? extension)
    {
        int bestOrder = -1;
        double bestQuality = -1;

        contentEncoding = null;
        extension = null;

        foreach (StringWithQualityHeaderValue value in requestHeaders.AcceptEncoding)
        {
            if (!_options.ContentEncoding.TryGetValue(value.Value, out (int, string) pair))
            {
                continue;
            }

            (int order, string ext) = pair;
            double quality = value.Quality ?? 1;

            if (quality == 0)
            {
                continue;
            }

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            // Save to compare as both doubles are never used in calculation
            if (quality == bestQuality)
            {
                if (order > bestOrder)
                {
                    bestOrder = order;
                    contentEncoding = value.Value;
                    extension = ext;
                }

                continue;
            }

            bestOrder = order;
            bestQuality = quality > bestQuality ? quality : bestQuality;
            contentEncoding = value.Value;
            extension = ext;
        }

        return contentEncoding.HasValue;
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

    private void SetCompressionMode(HttpContext context)
    {
        IHttpsCompressionFeature? responseCompressionFeature = context.Features.Get<IHttpsCompressionFeature>();
        if (responseCompressionFeature is not null)
        {
            responseCompressionFeature.Mode = _options.HttpsCompression;
        }
    }

    private static bool HasEndpointDelegate(HttpContext context)
    {
        return context.GetEndpoint()?.RequestDelegate is not null;
    }

    private static bool IsValidMethod(HttpContext context)
    {
        return HttpMethods.IsGet(context.Request.Method) ||
               HttpMethods.IsHead(context.Request.Method);
    }

    private static (EntityTagHeaderValue, DateTimeOffset) GetEtagAndLastModified(IFileInfo fileInfo)
    {
        DateTimeOffset last = fileInfo.LastModified;
        // Truncate to second precision
        DateTimeOffset lastModified = new DateTimeOffset(last.Year, last.Month, last.Day,
            last.Hour, last.Minute, last.Second, last.Offset).ToUniversalTime();

        long etagHash = lastModified.ToFileTime() ^ fileInfo.Length;
        EntityTagHeaderValue etag = new($"\"{Convert.ToString(etagHash, 16)}\"");

        return (etag, lastModified);
    }

    private static void SetContentEncoding(ResponseHeaders responseHeaders, StringSegment contentEncoding)
    {
        if (contentEncoding.HasValue)
        {
            responseHeaders.Headers.ContentEncoding = contentEncoding.Value;
        }
    }

    private static void SetStatusCode(HttpContext context, int statusCode)
    {
        context.Response.StatusCode = context.Response.StatusCode == StatusCodes.Status200OK
            ? statusCode
            : context.Response.StatusCode;
    }
}
