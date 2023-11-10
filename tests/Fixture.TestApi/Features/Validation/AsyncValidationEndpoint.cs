using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MinimalApiBuilder;

namespace Fixture.TestApi.Features.Validation;

internal partial class AsyncSingleValidationEndpoint : MinimalApiBuilderEndpoint
{
    private static async Task<IResult> HandleAsync(
        [FromServices] AsyncSingleValidationEndpoint endpoint,
        AsyncValidationRequest request,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        MultipartReader _ = new(context, endpoint);
        return TypedResults.Ok(endpoint.ValidationErrors);
    }
}

internal partial class AsyncMultipleValidationEndpoint : MinimalApiBuilderEndpoint
{
    private static async Task<IResult> HandleAsync(
        [FromServices] AsyncMultipleValidationEndpoint endpoint,
        [AsParameters] AsyncValidationParameters parameters,
        AsyncValidationRequest request,
        CancellationToken cancellationToken)
    {
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
