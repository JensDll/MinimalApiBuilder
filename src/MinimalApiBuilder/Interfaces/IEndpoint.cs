using FluentValidation.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MinimalApiBuilder;

public interface IEndpoint
{
#pragma warning disable IDE1006 // Naming Styles
    // ReSharper disable once InconsistentNaming
    public static abstract Delegate _auto_generated_Handler { get; }

    public static abstract void _auto_generated_Configure(RouteHandlerBuilder builder);
#pragma warning restore IDE1006 // Naming Styles

    public static abstract void Configure(RouteHandlerBuilder builder);

    public static BadRequest<ErrorDto> GetErrorResult(MinimalApiBuilderEndpoint endpoint, ValidationResult result)
    {
        foreach (ValidationFailure failure in result.Errors)
        {
            endpoint.ValidationErrors.Add(failure.ErrorMessage);
        }

        return ErrorResult(endpoint);
    }

    public static BadRequest<ErrorDto> GetErrorResult(MinimalApiBuilderEndpoint endpoint,
        params ValidationResult[] results)
    {
        foreach (ValidationResult result in results)
        {
            foreach (ValidationFailure failure in result.Errors)
            {
                endpoint.ValidationErrors.Add(failure.ErrorMessage);
            }
        }

        return ErrorResult(endpoint);
    }

    public static BadRequest<ErrorDto> ErrorResult(MinimalApiBuilderEndpoint endpoint) =>
        endpoint.ErrorResult("Validation failed");
}
