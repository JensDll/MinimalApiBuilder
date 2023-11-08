using System.Net;
using Microsoft.AspNetCore.Http;

namespace MinimalApiBuilder;

public abstract partial class MinimalApiBuilderEndpoint
{
    /// <summary>
    /// Sends an <see cref="ErrorDto" /> JSON response for the current request.
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext" /> of the current request.</param>
    /// <param name="message">
    /// The <see cref="ErrorDto.Message" />.
    /// </param>
    /// <param name="statusCode">
    /// The <see cref="ErrorDto.StatusCode" />.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken" /> used to cancel the operation.
    /// </param>
    /// <returns>
    /// The <see cref="Task" /> object representing the asynchronous operation.
    /// </returns>
    public Task SendErrorAsync(
        HttpContext httpContext,
        string message,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        CancellationToken cancellationToken = default)
    {
        httpContext.Response.StatusCode = (int)statusCode;
        return httpContext.Response.WriteAsJsonAsync(
            value: new ErrorDto
            {
                StatusCode = statusCode,
                Message = message,
                Errors = ValidationErrors
            },
            cancellationToken: cancellationToken);
    }
}
