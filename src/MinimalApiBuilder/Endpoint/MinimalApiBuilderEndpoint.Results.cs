using System.Net;
using Microsoft.AspNetCore.Http;

namespace MinimalApiBuilder;

public abstract partial class MinimalApiBuilderEndpoint
{
    public IResult ErrorResult(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return Results.BadRequest(new
        {
            StatusCode = statusCode,
            Message = message,
            Errors = ValidationErrors
        });
    }
}
