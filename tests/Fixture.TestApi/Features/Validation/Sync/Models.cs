using FluentValidation;

namespace Fixture.TestApi.Features.Validation.Sync;

public class SyncValidationRequest
{
    public required string Foo { get; init; }
};

public record struct SyncValidationParameters(int Bar);

public class RequestValidator : AbstractValidator<SyncValidationRequest>
{
    public RequestValidator()
    {
        RuleFor(request => request.Foo)
            .Must(value => value is not ("invalid" or "false" or "no"));
    }
}

public class ParametersValidator : AbstractValidator<SyncValidationParameters>
{
    public ParametersValidator()
    {
        RuleFor(parameters => parameters.Bar)
            .Must(value => value % 2 == 0);
    }
}
