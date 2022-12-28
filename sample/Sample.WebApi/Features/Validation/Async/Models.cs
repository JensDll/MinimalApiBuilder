using FluentValidation;

namespace Sample.WebApi.Features.Validation.Async;

public class Request
{
    public required string Foo { get; init; }

    public required List<int> Bar { get; init; }
}

public record struct Parameters(int A, int B);

public class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator(Serilog.ILogger logger)
    {
        logger.Information("Async request validator constructed");

        RuleFor(static request => request.Foo)
            .NotEmpty().WithMessage("Foo is required").MustAsync((_, _) => Task.FromResult(true));
    }
}

public class ParametersValidator : AbstractValidator<Parameters>
{
    public ParametersValidator()
    {
        RuleFor(static parameters => parameters.A)
            .GreaterThan(0).WithMessage("A must be greater than 0");

        RuleFor(static parameters => parameters.B)
            .GreaterThan(0).WithMessage("B must be greater than 0")
            .MustAsync(static async (b, cancellationToken) =>
            {
                await Task.Delay(0, cancellationToken);
                return b % 2 == 0;
            }).WithMessage("B must be even");
    }
}
