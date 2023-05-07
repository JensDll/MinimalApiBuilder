using FluentValidation;

namespace Fixture.TestApi.Features.Validation.Sync;

public class Request
{
    public required string Foo { get; init; }
};

public record struct Parameters(int Bar);

public class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator()
    {
        RuleFor(request => request.Foo).Must(foo => foo is not ("invalid" or "false" or "no"));
    }
}

public class ParametersValidator : AbstractValidator<Parameters>
{
    public ParametersValidator()
    {
        RuleFor(request => request.Bar).Must(bar => bar % 2 == 0);
    }
}
