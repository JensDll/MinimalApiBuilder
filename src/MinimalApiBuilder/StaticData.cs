using FluentValidation.Results;

namespace MinimalApiBuilder;

/// <summary>
/// Static data used by the Minimal API Builder generator.
/// </summary>
public static class StaticData
{
    /// <summary>
    /// A successful <see cref="FluentValidation.Results.ValidationResult" />.
    /// </summary>
    public static readonly ValidationResult SuccessValidationResult = new();

    /// <summary>
    /// A <see cref="SuccessValidationResult" /> <see cref="Task" />.
    /// </summary>
    public static readonly Task<ValidationResult> SuccessValidationResultTask =
        Task.FromResult(SuccessValidationResult);
}
