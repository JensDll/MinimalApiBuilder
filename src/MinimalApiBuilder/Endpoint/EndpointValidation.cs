using FluentValidation;
using FluentValidation.Results;

namespace MinimalApiBuilder;

public abstract partial class EndpointBase
{
    private List<string> ValidationErrors { get; } = new();

    protected void AddValidationError(string message)
    {
        ValidationErrors.Add(message);
    }

    protected bool HasValidationErrors => ValidationErrors.Count > 0;

    internal async Task<bool> ValidateAsync<T>(T request, CancellationToken cancellationToken)
    {
        var validator = (IValidator?)HttpContext.RequestServices.GetService(typeof(IValidator<T>));

        if (validator is null)
        {
            return true;
        }

        ValidationContext<T> validationContext = new(request);
        ValidationResult validationResult = await validator.ValidateAsync(validationContext, cancellationToken);

        if (validationResult.IsValid)
        {
            return true;
        }

        foreach (ValidationFailure failure in validationResult.Errors)
        {
            ValidationErrors.Add(failure.ErrorMessage);
        }

        return false;
    }
}
