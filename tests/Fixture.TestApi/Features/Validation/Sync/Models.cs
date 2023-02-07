using FluentValidation;

namespace Sample.WebApi.Features.Validation.Sync;

public class SyncValidationRequest
{
    public required string Foo { get; init; }
};

public record struct SyncValidationParameters(int Bar);

public class SyncValidationRequestValidator : AbstractValidator<SyncValidationRequest>
{
    public SyncValidationRequestValidator()
    {
        RuleFor(request => request.Foo).Must(foo => foo is not ("invalid" or "false" or "no"));
    }
}

public class SyncValidationParametersValidator : AbstractValidator<SyncValidationParameters>
{
    public SyncValidationParametersValidator()
    {
        RuleFor(request => request.Bar).Must(bar => bar % 2 == 0);
    }
}
