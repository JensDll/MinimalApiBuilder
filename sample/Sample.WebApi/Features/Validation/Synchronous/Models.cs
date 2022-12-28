using FluentValidation;

namespace Sample.WebApi.Features.Validation.Synchronous;

public class SyncValidationRequest
{
    public required string Foo { get; init; }
};

public record struct SyncValidationParameters(int Bar);

public class SyncValidationRequestValidator : AbstractValidator<SyncValidationRequest>
{
    public SyncValidationRequestValidator()
    {
        RuleFor(request => request.Foo.Length).GreaterThan(1);
    }
}

public class SyncValidationParametersValidator : AbstractValidator<SyncValidationParameters>
{
    public SyncValidationParametersValidator()
    {
        RuleFor(request => request.Bar).Must(bar => bar % 2 == 0);
    }
}
