using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MinimalApiBuilder;

namespace Fixture.TestApi.Features.Validation.Async;

public class Request
{
    public required string Foo { get; init; }
}

public record struct Parameters(int Bar);

[RegisterValidator(ServiceLifetime.Transient)]
public class RequestValidator : AbstractValidator<Request>
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

public class ParametersValidator : AbstractValidator<Parameters>
{
    public ParametersValidator()
    {
        RuleFor(static request => request.Bar)
            .MustAsync(static async (bar, cancellationToken) =>
            {
                await Task.Delay(0, cancellationToken);
                return bar % 2 == 0;
            });
    }
}
