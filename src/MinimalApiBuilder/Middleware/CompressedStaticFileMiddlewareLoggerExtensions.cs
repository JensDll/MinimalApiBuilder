using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MinimalApiBuilder.Middleware;

internal static partial class CompressedStaticFileMiddlewareLoggerExtensions
{
    private static readonly Func<ILogger, PathString, IDisposable?> s_middleware =
        LoggerMessage.DefineScope<PathString>($"{nameof(CompressedStaticFileMiddleware)}: {{Path}}");

    internal static IDisposable? CompressedStaticFileMiddleware(this ILogger logger, PathString path)
    {
        return s_middleware(logger, path);
    }

    [LoggerMessage(0, LogLevel.Debug, "Skipping as the request already matches an endpoint")]
    internal static partial void EndpointMatched(this ILogger logger);

    [LoggerMessage(1, LogLevel.Debug, "Skipping {Method} requests as only GET or HEAD requests are supported")]
    internal static partial void InvalidMethod(this ILogger logger, string method);

    [LoggerMessage(2, LogLevel.Debug,
        "Skipping as the request path does not start with the configured prefix {Prefix}")]
    internal static partial void PathMismatch(this ILogger logger, PathString prefix);

    [LoggerMessage(3, LogLevel.Debug, "Skipping as there is no valid content type mapping")]
    internal static partial void InvalidContentType(this ILogger logger);

    [LoggerMessage(4, LogLevel.Debug, "Skipping as the requested file does not exist")]
    internal static partial void FileDoesNotExist(this ILogger logger);

    [LoggerMessage(5, LogLevel.Debug, "Sending file {Path}")]
    public static partial void SendingFile(this ILogger logger, string path);

    [LoggerMessage(6, LogLevel.Debug, "Sending file {Path} range from {Start} to {End}")]
    public static partial void SendingRange(this ILogger logger, long start, long end, string path);

    [LoggerMessage(7, LogLevel.Debug, "The file transmission was cancelled")]
    internal static partial void SendFileCancelled(this ILogger logger, OperationCanceledException e);

    [LoggerMessage(8, LogLevel.Debug, "The file {Path} was not modified")]
    public static partial void NotModified(this ILogger logger, string path);

    [LoggerMessage(9, LogLevel.Debug, "Precondition for {Path} failed")]
    public static partial void PreconditionFailed(this ILogger logger, string path);

    [LoggerMessage(10, LogLevel.Debug, "If-Range precondition for {Path} failed")]
    public static partial void IfRangePreconditionFailed(this ILogger logger, string path);

    [LoggerMessage(20, LogLevel.Warning,
        "The WebRootPath was not found: {WebRootPath}. Compressed static files may be unavailable")]
    public static partial void WebRootPathNotFound(this ILogger logger, string webRootPath);

    [LoggerMessage(21, LogLevel.Warning, "Range {Range} not satisfiable for {Path}")]
    public static partial void RangeNotSatisfiable(this ILogger logger, string? range, string path);
}
