using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MinimalApiBuilder;

namespace Fixture.TestApi.Features.Validation.Async;

internal class AsyncValidationRequest
{
    public required string Foo { get; init; }
}

internal record struct AsyncValidationParameters(int Bar);

[RegisterValidator(ServiceLifetime.Transient)]
internal class RequestValidator : AbstractValidator<AsyncValidationRequest>
{
    public RequestValidator()
    {
        RuleFor(static request => request.Foo)
            .MustAsync(static async (foo, cancellationToken) =>
            {
                await Task.CompletedTask;
                return foo is not ("invalid" or "false" or "no");
            });
    }
}

internal class ParametersValidator : AbstractValidator<AsyncValidationParameters>
{
    public ParametersValidator()
    {
        RuleFor(static parameters => parameters.Bar)
            .MustAsync(static async (value, cancellationToken) =>
            {
                await Task.CompletedTask;
                return value % 2 == 0;
            });
    }
}
