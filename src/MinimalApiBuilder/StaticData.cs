using FluentValidation.Results;

namespace MinimalApiBuilder;

/// <summary>
/// Static data used by the minimal API builder generator.
/// </summary>
public static class StaticData
{
    /// <summary>
    /// A successful <see cref="ValidationResult" />.
    /// </summary>
    public static readonly ValidationResult SuccessValidationResult = new();

    /// <summary>
    /// A <see cref="SuccessValidationResult" /> <see cref="Task" />.
    /// </summary>
    public static readonly Task<ValidationResult> SuccessValidationResultTask =
        Task.FromResult(SuccessValidationResult);
}
