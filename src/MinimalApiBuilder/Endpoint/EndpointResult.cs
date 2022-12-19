using Microsoft.AspNetCore.Http;

namespace MinimalApiBuilder;

public abstract partial class EndpointBase
{
    protected IResult ErrorResult(string message, int statusCode)
    {
        return Results.BadRequest(new
        {
            StatusCode = statusCode,
            Message = message,
            Errors = ValidationErrors
        });
    }

    protected IResult ErrorResult(string message)
    {
        return Results.BadRequest(new
        {
            StatusCode = 400,
            Message = message,
            Errors = ValidationErrors
        });
    }
}
