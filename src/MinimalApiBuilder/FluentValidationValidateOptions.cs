using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Options;

namespace MinimalApiBuilder;

/// <summary>
/// Options validator using FluentValidation.
/// </summary>
/// <typeparam name="TOptions">The type of options being validated.</typeparam>
public class FluentValidationValidateOptions<TOptions> : IValidateOptions<TOptions>
    where TOptions : class
{
    private readonly IValidator<TOptions> _validator;

    /// <summary>
    /// Initializes a new instance of <see cref="FluentValidationValidateOptions{TOptions}" />.
    /// </summary>
    /// <param name="validator">The options validator.</param>
    public FluentValidationValidateOptions(IValidator<TOptions> validator)
    {
        _validator = validator;
    }

    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? name, TOptions options)
    {
        ValidationResult result = _validator.Validate(options);

        if (result.IsValid)
        {
            return ValidateOptionsResult.Success;
        }

        IEnumerable<string> errors = result.Errors
            .Where(static failure => failure.Severity == Severity.Error)
            .Select(static failure => failure.ErrorMessage);

        return ValidateOptionsResult.Fail(errors);
    }
}
