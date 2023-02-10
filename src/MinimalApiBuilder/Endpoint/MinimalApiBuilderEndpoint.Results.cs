using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MinimalApiBuilder;

public abstract partial class MinimalApiBuilderEndpoint
{
    public BadRequest<ErrorDto> ErrorResult(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return TypedResults.BadRequest(new ErrorDto()
        {
            StatusCode = statusCode,
            Message = message,
            Errors = ValidationErrors
        });
    }
}
