using FluentValidation;
using MinimalApiBuilder;

namespace Sample.WebApi.Features.Validation.Async;

public class Request
{
    public required string Foo { get; init; }

    public required List<int> Bar { get; init; }
}

public record struct Parameters(int A, int B);

public class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator()
    {
        RuleFor(request => request.Foo)
            .NotEmpty().WithMessage("Foo is required");
    }
}

[RegisterValidatorAsDependency]
public class ParametersValidator : AbstractValidator<Parameters>
{
    public ParametersValidator()
    {
        RuleFor(parameters => parameters.A)
            .GreaterThan(0).WithMessage("A must be greater than 0");
        RuleFor(parameters => parameters.B)
            .GreaterThan(0).WithMessage("B must be greater than 0");
    }
}
