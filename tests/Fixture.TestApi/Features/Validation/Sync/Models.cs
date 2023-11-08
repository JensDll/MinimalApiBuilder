using FluentValidation;

namespace Fixture.TestApi.Features.Validation.Sync;

internal class SyncValidationRequest
{
    public required string Foo { get; init; }
};

internal record struct SyncValidationParameters(int Bar);

internal class RequestValidator : AbstractValidator<SyncValidationRequest>
{
    public RequestValidator()
    {
        RuleFor(request => request.Foo)
            .Must(value => value is not ("invalid" or "false" or "no"));
    }
}

internal class ParametersValidator : AbstractValidator<SyncValidationParameters>
{
    public ParametersValidator()
    {
        RuleFor(parameters => parameters.Bar)
            .Must(value => value % 2 == 0);
    }
}
