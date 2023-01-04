using Microsoft.AspNetCore.Http;

namespace MinimalApiBuilder;

public static class ResultsExtensions
{
    public static IResult BodyWriterStream(this IResultExtensions resultExtensions,
        Func<Stream, Task> streamWriterCallback,
        string? contentType = null,
        string? fileDownloadName = null)
    {
        ArgumentNullException.ThrowIfNull(resultExtensions);
        return new BodyWriterStreamResult(streamWriterCallback, contentType, fileDownloadName);
    }
}
