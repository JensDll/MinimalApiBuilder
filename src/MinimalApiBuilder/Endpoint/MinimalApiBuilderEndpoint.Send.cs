using System.Net;
using Microsoft.AspNetCore.Http;

namespace MinimalApiBuilder;

public abstract partial class MinimalApiBuilderEndpoint
{
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
