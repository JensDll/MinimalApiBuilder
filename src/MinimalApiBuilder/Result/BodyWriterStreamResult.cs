using Microsoft.AspNetCore.Http;

namespace MinimalApiBuilder;

internal sealed class BodyWriterStreamResult : IResult
{
    private readonly Func<Stream, Task> _streamWriterCallback;
    private readonly string _contentType;
    private readonly string _fileDownloadName;

    public BodyWriterStreamResult(Func<Stream, Task> streamWriterCallback,
        string contentType,
        string fileDownloadName)
    {
        _streamWriterCallback = streamWriterCallback;
        _contentType = contentType;
        _fileDownloadName = fileDownloadName;
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.ContentType = _contentType;
        httpContext.Response.Headers.Add("Content-Disposition", $"attachment; filename={_fileDownloadName}");
        return _streamWriterCallback(httpContext.Response.BodyWriter.AsStream());
    }
}
