using System.Net;

namespace MinimalApiBuilder;

/// <summary>
/// The response object to validation and model binding failures.
/// </summary>
public class ErrorDto
{
    /// <summary>
    /// The response <see cref="HttpStatusCode" />.
    /// </summary>
    public required HttpStatusCode StatusCode { get; init; }

    /// <summary>
    /// The response friendly message.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Any additional error messages.
    /// </summary>
    public required IEnumerable<string> Errors { get; init; }
}
