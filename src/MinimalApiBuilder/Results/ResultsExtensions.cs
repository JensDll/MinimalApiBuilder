using Microsoft.AspNetCore.Http;

namespace MinimalApiBuilder;

/// <summary>
/// Extension methods for <see cref="IResultExtensions" />.
/// </summary>
public static class ResultsExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="resultExtensions"></param>
    /// <param name="streamWriterCallback"></param>
    /// <param name="contentType"></param>
    /// <param name="fileDownloadName"></param>
    /// <returns></returns>
    public static IResult BodyWriterStream(this IResultExtensions resultExtensions,
        Func<Stream, Task> streamWriterCallback,
        string? contentType = null,
        string? fileDownloadName = null)
    {
        ArgumentNullException.ThrowIfNull(resultExtensions);
        return new BodyWriterStreamResult(streamWriterCallback, contentType, fileDownloadName);
    }
}
