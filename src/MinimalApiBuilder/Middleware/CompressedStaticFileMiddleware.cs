using System.Diagnostics;
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
/// </summary>
public class CompressedStaticFileMiddleware : IMiddleware
{
    private readonly CompressedStaticFileOptions _options;
    private readonly ILogger<CompressedStaticFileMiddleware> _logger;
    private readonly IFileProvider _fileProvider;
    private readonly IContentTypeProvider _contentTypeProvider;

    /// <summary>
    /// </summary>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    public CompressedStaticFileMiddleware(IOptions<CompressedStaticFileOptions> options,
        ILogger<CompressedStaticFileMiddleware> logger)
    {
        _options = options.Value;
        _logger = logger;
        _fileProvider = _options.FileProvider!;
        _contentTypeProvider = _options.ContentTypeProvider;
    }

    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        PathString requestPath = context.Request.Path;
        RequestHeaders requestHeaders = context.Request.GetTypedHeaders();
        ResponseHeaders responseHeaders = context.Response.GetTypedHeaders();

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
            return Task.CompletedTask;
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
                    return SendFileAsync(staticFileResponseContext, subPath);
                }

                if (!RangeHelper.TryParseRange(context, requestHeaders, fileInfo.Length, out (long, long)? range))
                {
                    return SendFileAsync(staticFileResponseContext, subPath);
                }

                if (range is null)
                {
                    context.Response.StatusCode = StatusCodes.Status416RangeNotSatisfiable;
                    responseHeaders.ContentRange = new ContentRangeHeaderValue(fileInfo.Length);
                    _logger.RangeNotSatisfiable(context.Request.Headers.Range, subPath);
                    return Task.CompletedTask;
                }

                (long start, long end) = range.Value;
                long count = end - start + 1;

                responseHeaders.ContentRange = new ContentRangeHeaderValue(start, end, fileInfo.Length);
                context.Response.StatusCode = StatusCodes.Status206PartialContent;
                context.Response.ContentLength = count;
                _logger.SendingRange(start, end, subPath);
                return SendFileAsync(staticFileResponseContext, start, count);
            case PreconditionState.NotModified:
                responseHeaders.LastModified = lastModified;
                responseHeaders.ETag = etag;
                responseHeaders.Headers.AcceptRanges = "bytes";
                SetContentEncoding(responseHeaders, contentEncoding);
                context.Response.StatusCode = StatusCodes.Status304NotModified;
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
                context.Response.StatusCode = StatusCodes.Status412PreconditionFailed;
                _logger.PreconditionFailed(subPath);
                _options.OnPrepareResponse(staticFileResponseContext);
#if NET8_0_OR_GREATER
                return _options.OnPrepareResponseAsync(staticFileResponseContext);
#else
                return Task.CompletedTask;
#endif
            default:
                NotImplementedException exception = new($"Unexpected precondition state value: {precondition}");
                Debug.Fail(exception.ToString());
                throw exception;
        }
    }

    private IFileInfo GetFileInfo(RequestHeaders requestHeaders, string subPath, out StringSegment contentEncoding)
    {
        IFileInfo fileInfo = _fileProvider.GetFileInfo(subPath);

        if (!TryGetContentEncoding(requestHeaders, out contentEncoding))
        {
            return fileInfo;
        }

        IFileInfo compressedFileInfo = _fileProvider.GetFileInfo($"{subPath}.{contentEncoding}");

        if (compressedFileInfo.Exists)
        {
            return compressedFileInfo;
        }

        contentEncoding = null;

        return fileInfo;
    }

    private async Task SendFileAsync(StaticFileResponseContext context, long offset, long count)
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
    }

    private Task SendFileAsync(StaticFileResponseContext context, string subPath)
    {
        context.Context.Response.ContentLength = context.File.Length;
        _logger.SendingFile(subPath);
        return SendFileAsync(context, 0, context.File.Length);
    }

    private bool TryGetContentEncoding(RequestHeaders requestHeaders, out StringSegment contentEncoding)
    {
        int bestOrder = -1;
        double bestQuality = -1;

        contentEncoding = null;

        foreach (StringWithQualityHeaderValue value in requestHeaders.AcceptEncoding)
        {
            if (!_options.ContentEncodingOrder.TryGetValue(value.Value, out int order))
            {
                continue;
            }

            double quality = value.Quality ?? 1;

            if (quality == 0)
            {
                continue;
            }

            // Save to compare as both doubles are never used in calculation
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (quality == bestQuality)
            {
                if (order > bestOrder)
                {
                    bestOrder = order;
                    contentEncoding = value.Value;
                }

                continue;
            }

            bestOrder = order;
            bestQuality = quality > bestQuality ? quality : bestQuality;
            contentEncoding = value.Value;
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
}
