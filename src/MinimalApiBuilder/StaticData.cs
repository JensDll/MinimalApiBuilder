using FluentValidation.Results;

namespace MinimalApiBuilder;

public static class StaticData
{
    public static readonly ValidationResult SuccessValidationResult = new();

    public static readonly Task<ValidationResult> SuccessValidationResultTask =
        Task.FromResult(SuccessValidationResult);
}
