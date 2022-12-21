using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace MinimalApiBuilder;

public abstract partial class Endpoint<TEndpoint>
    where TEndpoint : EndpointBase, IEndpoint
{
    protected void AddValidationError(string message)
    {
        ValidationErrors.Add(message);
    }

    protected bool HasValidationErrors => ValidationErrors.Count > 0;

    protected void Validate<T1, T1Validator>(RouteHandlerBuilder builder)
        where T1Validator : IValidator<T1>, new()
    {
        int t1Index = TEndpoint.ArgumentPositions[typeof(T1)];
        int endpointIndex = TEndpoint.ArgumentPositions[typeof(TEndpoint)];

        T1Validator t1Validator = new();

        builder.AddEndpointFilter((invocationContext, next) =>
        {
            T1 t1 = invocationContext.GetArgument<T1>(t1Index);
            TEndpoint endpoint = invocationContext.GetArgument<TEndpoint>(endpointIndex);

            ValidationResult result = t1Validator.Validate(t1);

            return result.IsValid
                ? next(invocationContext)
                : ValueTask.FromResult<object?>(GetErrorResult(endpoint, result));
        });
    }

    protected void Validate<T1, T1Validator, T2, T2Validator>(RouteHandlerBuilder builder)
        where T1Validator : IValidator<T1>, new()
        where T2Validator : IValidator<T2>, new()
    {
        int t1Index = TEndpoint.ArgumentPositions[typeof(T1)];
        int t2Index = TEndpoint.ArgumentPositions[typeof(T2)];
        int endpointIndex = TEndpoint.ArgumentPositions[typeof(TEndpoint)];

        T1Validator t1Validator = new();
        T2Validator t2Validator = new();

        builder.AddEndpointFilter((invocationContext, next) =>
        {
            T1 t1 = invocationContext.GetArgument<T1>(t1Index);
            T2 t2 = invocationContext.GetArgument<T2>(t2Index);
            TEndpoint endpoint = invocationContext.GetArgument<TEndpoint>(endpointIndex);

            ValidationResult[] results = { t1Validator.Validate(t1), t2Validator.Validate(t2) };

            return results.Any(Invalid)
                ? ValueTask.FromResult<object?>(GetErrorResult(endpoint, results))
                : next(invocationContext);
        });
    }

    protected void Validate<T1, T1Validator, T2, T2Validator, T3, T3Validator>(RouteHandlerBuilder builder)
        where T1Validator : IValidator<T1>, new()
        where T2Validator : IValidator<T2>, new()
        where T3Validator : IValidator<T3>, new()
    {
        int t1Index = TEndpoint.ArgumentPositions[typeof(T1)];
        int t2Index = TEndpoint.ArgumentPositions[typeof(T2)];
        int t3Index = TEndpoint.ArgumentPositions[typeof(T3)];
        int endpointIndex = TEndpoint.ArgumentPositions[typeof(TEndpoint)];

        T1Validator t1Validator = new();
        T2Validator t2Validator = new();
        T3Validator t3Validator = new();

        builder.AddEndpointFilter((invocationContext, next) =>
        {
            T1 t1 = invocationContext.GetArgument<T1>(t1Index);
            T2 t2 = invocationContext.GetArgument<T2>(t2Index);
            T3 t3 = invocationContext.GetArgument<T3>(t3Index);
            TEndpoint endpoint = invocationContext.GetArgument<TEndpoint>(endpointIndex);

            ValidationResult[] results =
                { t1Validator.Validate(t1), t2Validator.Validate(t2), t3Validator.Validate(t3) };

            return results.Any(Invalid)
                ? ValueTask.FromResult<object?>(GetErrorResult(endpoint, results))
                : next(invocationContext);
        });
    }

    protected void ValidateAsync<T1, T1Validator>(RouteHandlerBuilder builder)
        where T1Validator : IValidator<T1>, new()
    {
        int t1Index = TEndpoint.ArgumentPositions[typeof(T1)];
        int endpointIndex = TEndpoint.ArgumentPositions[typeof(TEndpoint)];

        T1Validator t1Validator = new();

        builder.AddEndpointFilter(async (invocationContext, next) =>
        {
            T1 t1 = invocationContext.GetArgument<T1>(t1Index);
            TEndpoint endpoint = invocationContext.GetArgument<TEndpoint>(endpointIndex);

            ValidationResult result = await t1Validator.ValidateAsync(t1);

            return result.IsValid ? await next(invocationContext) : GetErrorResult(endpoint, result);
        });
    }

    protected void ValidateAsync<T1, T1Validator, T2, T2Validator>(RouteHandlerBuilder builder)
        where T1Validator : IValidator<T1>, new()
        where T2Validator : IValidator<T2>, new()
    {
        int t1Index = TEndpoint.ArgumentPositions[typeof(T1)];
        int t2Index = TEndpoint.ArgumentPositions[typeof(T2)];
        int endpointIndex = TEndpoint.ArgumentPositions[typeof(TEndpoint)];

        T1Validator t1Validator = new();
        T2Validator t2Validator = new();

        builder.AddEndpointFilter(async (invocationContext, next) =>
        {
            T1 t1 = invocationContext.GetArgument<T1>(t1Index);
            T2 t2 = invocationContext.GetArgument<T2>(t2Index);
            TEndpoint endpoint = invocationContext.GetArgument<TEndpoint>(endpointIndex);

            ValidationResult[] results =
                await Task.WhenAll(t1Validator.ValidateAsync(t1), t2Validator.ValidateAsync(t2));

            return results.Any(Invalid) ? GetErrorResult(endpoint, results) : await next(invocationContext);
        });
    }

    protected void ValidateAsync<T1, T1Validator, T2, T2Validator, T3, T3Validator>(RouteHandlerBuilder builder)
        where T1Validator : IValidator<T1>, new()
        where T2Validator : IValidator<T2>, new()
        where T3Validator : IValidator<T3>, new()
    {
        int t1Index = TEndpoint.ArgumentPositions[typeof(T1)];
        int t2Index = TEndpoint.ArgumentPositions[typeof(T2)];
        int t3Index = TEndpoint.ArgumentPositions[typeof(T3)];
        int endpointIndex = TEndpoint.ArgumentPositions[typeof(TEndpoint)];

        T1Validator t1Validator = new();
        T2Validator t2Validator = new();
        T3Validator t3Validator = new();

        builder.AddEndpointFilter(async (invocationContext, next) =>
        {
            T1 t1 = invocationContext.GetArgument<T1>(t1Index);
            T2 t2 = invocationContext.GetArgument<T2>(t2Index);
            T3 t3 = invocationContext.GetArgument<T3>(t3Index);
            TEndpoint endpoint = invocationContext.GetArgument<TEndpoint>(endpointIndex);

            ValidationResult[] results =
                await Task.WhenAll(t1Validator.ValidateAsync(t1), t2Validator.ValidateAsync(t2),
                    t3Validator.ValidateAsync(t3));

            return results.Any(Invalid) ? GetErrorResult(endpoint, results) : await next(invocationContext);
        });
    }

    private static bool Invalid(ValidationResult result) => !result.IsValid;

    private static IResult GetErrorResult(TEndpoint endpoint, ValidationResult result)
    {
        foreach (ValidationFailure failure in result.Errors)
        {
            endpoint.ValidationErrors.Add(failure.ErrorMessage);
        }

        return ErrorResult(endpoint);
    }

    private static IResult GetErrorResult(TEndpoint endpoint, params ValidationResult[] results)
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

    private static IResult ErrorResult(TEndpoint endpoint) => endpoint.ErrorResult("Validation failed");
}
