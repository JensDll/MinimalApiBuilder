using System.Net;
using Microsoft.AspNetCore.Http;

namespace MinimalApiBuilder;

public abstract partial class Endpoint<TEndpoint>
    where TEndpoint : EndpointBase, IEndpoint
{
    public IResult ErrorResult(string message)
    {
        return Results.BadRequest(new
        {
            StatusCode = HttpStatusCode.BadRequest,
            Message = message,
            Errors = ValidationErrors
        });
    }

    public IResult ErrorResult(string message, HttpStatusCode statusCode)
    {
        return Results.BadRequest(new
        {
            StatusCode = statusCode,
            Message = message,
            Errors = ValidationErrors
        });
    }
}
