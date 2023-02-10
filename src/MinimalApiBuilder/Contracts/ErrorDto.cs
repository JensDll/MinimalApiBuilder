using System.Net;

namespace MinimalApiBuilder;

public class ErrorDto
{
    public required HttpStatusCode StatusCode { get; init; }

    public required string Message { get; init; }

    public required IEnumerable<string> Errors { get; init; }
}
