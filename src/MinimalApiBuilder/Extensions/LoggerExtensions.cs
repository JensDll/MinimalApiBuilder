using Microsoft.Extensions.Logging;

namespace MinimalApiBuilder;

internal static class LoggerExtensions
{
    private static readonly Action<ILogger, string, Exception?> s_failedToCreateMultipartReader =
        LoggerMessage.Define<string>(
            LogLevel.Warning,
            new EventId(0, nameof(FailedToCreateMultipartReader)),
            "Failed to create MultipartReader: {Message}");

    internal static void FailedToCreateMultipartReader(this ILogger logger, string message)
    {
        s_failedToCreateMultipartReader(logger, message, null);
    }
}
