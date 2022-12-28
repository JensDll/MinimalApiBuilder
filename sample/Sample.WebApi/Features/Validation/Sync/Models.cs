using FluentValidation;

namespace Sample.WebApi.Features.Validation.Sync;

public record SyncRequest(string Something);

public class RequestValidator : AbstractValidator<SyncRequest>
{
    public RequestValidator()
    {
        RuleFor(request => request.Something).NotEmpty();
    }
}
