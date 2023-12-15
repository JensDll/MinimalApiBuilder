using FluentValidation.Results;

namespace MinimalApiBuilder.Generator;

/// <summary>
/// Static helper used by the minimal API builder generator.
/// </summary>
public static class StaticHelper
{
    /// <summary>
    /// A <see cref="ValidationResult" /> without errors.
    /// </summary>
    public static readonly ValidationResult SuccessValidationResult = new();

    /// <summary>
    /// A completed <see cref="ValidationResult" /> <see cref="Task{TResult}" /> without errors.
    /// </summary>
    public static readonly Task<ValidationResult> SuccessValidationResultTask =
        Task.FromResult(SuccessValidationResult);

    /// <summary>
    /// Gets the <see cref="ValidationResult" /> errors as a <see cref="ValidationFailure.PropertyName" />
    /// grouped dictionary of error messages.
    /// </summary>
    /// <param name="result">The <see cref="ValidationResult" /> with errors.</param>
    /// <returns></returns>
    public static Dictionary<string, string[]> GetErrors(ValidationResult result)
    {
        return result.Errors
            .GroupBy(static failure => failure.PropertyName)
            .ToDictionary(static grouping => grouping.Key,
                static grouping => grouping.Select(static failure => failure.ErrorMessage).ToArray());
    }

    /// <summary>
    /// Gets every <see cref="ValidationResult" /> error as a <see cref="ValidationFailure.PropertyName" />
    /// grouped dictionary of error messages.
    /// </summary>
    /// <param name="results">The sequence of <see cref="ValidationResult" /> with errors.</param>
    /// <returns></returns>
    public static Dictionary<string, string[]> GetErrors(IEnumerable<ValidationResult> results)
    {
        return results.SelectMany(static result => result.Errors)
            .GroupBy(static failure => failure.PropertyName)
            .ToDictionary(static grouping => grouping.Key,
                static grouping => grouping.Select(static failure => failure.ErrorMessage).ToArray());
    }
}
