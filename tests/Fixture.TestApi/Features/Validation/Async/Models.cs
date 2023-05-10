using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MinimalApiBuilder;

namespace Fixture.TestApi.Features.Validation.Async;

public class AsyncValidationRequest
{
    public required string Foo { get; init; }
}

public record struct AsyncValidationParameters(int Bar);

[RegisterValidator(ServiceLifetime.Transient)]
public class RequestValidator : AbstractValidator<AsyncValidationRequest>
{
    public RequestValidator()
    {
        RuleFor(static request => request.Foo)
            .MustAsync(static async (foo, cancellationToken) =>
            {
                await Task.Delay(0, cancellationToken);
                return foo is not ("invalid" or "false" or "no");
            });
    }
}

public class ParametersValidator : AbstractValidator<AsyncValidationParameters>
{
    public ParametersValidator()
    {
        RuleFor(static parameters => parameters.Bar)
            .MustAsync(static async (value, cancellationToken) =>
            {
                await Task.Delay(0, cancellationToken);
                return value % 2 == 0;
            });
    }
}
