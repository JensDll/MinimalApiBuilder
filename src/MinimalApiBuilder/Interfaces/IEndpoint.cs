using System.Net;
using FluentValidation.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MinimalApiBuilder;

public interface IEndpoint
{
#pragma warning disable CA1707
#pragma warning disable IDE1006
    static abstract Delegate _auto_generated_Handler { get; }

    static abstract void _auto_generated_Configure(RouteHandlerBuilder builder);
#pragma warning restore IDE1006
#pragma warning restore CA1707

    static abstract void Configure(RouteHandlerBuilder builder);

    static BadRequest<ErrorDto> GetValidationErrorResult(MinimalApiBuilderEndpoint endpoint, ValidationResult result)
    {
        foreach (ValidationFailure failure in result.Errors)
        {
            endpoint.ValidationErrors.Add(failure.ErrorMessage);
        }

        return ValidationErrorResult(endpoint);
    }

    static BadRequest<ErrorDto> GetValidationErrorResult(MinimalApiBuilderEndpoint endpoint,
        params ValidationResult[] results)
    {
        foreach (ValidationResult result in results)
        {
            foreach (ValidationFailure failure in result.Errors)
            {
                endpoint.ValidationErrors.Add(failure.ErrorMessage);
            }
        }

        return ValidationErrorResult(endpoint);
    }

    static BadRequest<ErrorDto> ValidationErrorResult(MinimalApiBuilderEndpoint endpoint) =>
        TypedResults.BadRequest(new ErrorDto
        {
            StatusCode = HttpStatusCode.BadRequest,
            Message = "Validation failed",
            Errors = endpoint.ValidationErrors
        });
}
