using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MinimalApiBuilder;

namespace Fixture.TestApi.Features.Validation;

internal partial class AsyncSingleValidationEndpoint : MinimalApiBuilderEndpoint
{
    public static async Task<IResult> Handle(
        [FromServices] AsyncSingleValidationEndpoint endpoint,
        Serilog.ILogger logger,
        AsyncValidationRequest request,
        HttpContext context)
    {
        logger.Information("Request: {Request}", request);
        await Task.CompletedTask;
        MultipartReader _ = new(context, endpoint);
        return TypedResults.Ok(endpoint.ValidationErrors);
    }
}

internal partial class AsyncMultipleValidationEndpoint : MinimalApiBuilderEndpoint
{
    public static async Task<IResult> Handle(
        [AsParameters] AsyncValidationParameters parameters,
        AsyncValidationRequest request,
        Serilog.ILogger logger)
    {
        logger.Information("Parameters: {Parameters}", parameters);
        logger.Information("Request: {Request}", request);
        await Task.CompletedTask;
        return TypedResults.Ok();
    }
}

internal record struct AsyncValidationParameters(int Value);

internal record AsyncValidationRequest(string Value);

[RegisterValidator(ServiceLifetime.Transient)]
internal class AsyncValidationRequestValidator : AbstractValidator<AsyncValidationRequest>
{
    public AsyncValidationRequestValidator()
    {
        RuleFor(static request => request.Value).MustAsync(static async (value, _) =>
        {
            await Task.CompletedTask;
            return bool.TryParse(value, out bool result) && result;
        }).WithMessage("Property '{PropertyName}' with value '{PropertyValue}' must be true.");
    }
}

internal class AsyncValidationParametersValidator : AbstractValidator<AsyncValidationParameters>
{
    public AsyncValidationParametersValidator()
    {
        RuleFor(static parameters => parameters.Value).MustAsync(static async (value, _) =>
        {
            await Task.CompletedTask;
            return value % 2 == 0;
        }).WithMessage("Parameter '{PropertyName}' with value '{PropertyValue}' must be even.");
    }
}
