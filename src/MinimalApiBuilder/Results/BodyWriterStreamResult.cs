using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace MinimalApiBuilder;

internal sealed class BodyWriterStreamResult : IResult
{
    private readonly string? _contentType;
    private readonly string? _fileDownloadName;
    private readonly Func<Stream, Task> _streamWriterCallback;

    public BodyWriterStreamResult(
        Func<Stream, Task> streamWriterCallback,
        string? contentType,
        string? fileDownloadName)
    {
        _streamWriterCallback = streamWriterCallback;
        _contentType = contentType;
        _fileDownloadName = fileDownloadName;
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.ContentType = _contentType;

        if (!string.IsNullOrEmpty(_fileDownloadName))
        {
            var contentDisposition = new ContentDispositionHeaderValue("attachment");
            contentDisposition.SetHttpFileName(_fileDownloadName);
            httpContext.Response.Headers.ContentDisposition = contentDisposition.ToString();
        }

        return _streamWriterCallback(httpContext.Response.BodyWriter.AsStream());
    }
}
