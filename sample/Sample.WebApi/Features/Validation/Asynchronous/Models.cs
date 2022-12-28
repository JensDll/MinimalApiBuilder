using FluentValidation;

namespace Sample.WebApi.Features.Validation.Asynchronous;

public class AsyncValidationRequest
{
    public required string Foo { get; init; }
}

public record struct AsyncValidationParameters(int Bar);

public class AsyncValidationRequestValidator : AbstractValidator<AsyncValidationRequest>
{
    public AsyncValidationRequestValidator()
    {
        RuleFor(static request => request.Foo)
            .MustAsync(static async (foo, cancellationToken) =>
            {
                await Task.Delay(0, cancellationToken);
                return foo.Length > 1;
            });
    }
}

public class AsyncValidationParametersValidator : AbstractValidator<AsyncValidationParameters>
{
    public AsyncValidationParametersValidator()
    {
        RuleFor(static request => request.Bar)
            .MustAsync(static async (bar, cancellationToken) =>
            {
                await Task.Delay(0, cancellationToken);
                return bar % 2 == 0;
            });
    }
}
