namespace MinimalApiBuilder;

/// <summary>
/// The base class of all minimal API builder endpoints.
/// </summary>
public abstract class MinimalApiBuilderEndpoint
{
    /// <summary>
    /// The current validation errors of the request.
    /// </summary>
    protected internal Dictionary<string, string[]> ValidationErrors { get; } = new();

    /// <summary>
    /// A <see cref="bool" /> property indicating validation errors.
    /// </summary>
    public bool HasValidationError => ValidationErrors.Count > 0;

    /// <summary>
    /// Adds a new validation error to the <see cref="ValidationErrors" />
    /// using the default <see cref="string.Empty" /> group.
    /// </summary>
    /// <param name="message">The error message.</param>
    public void AddValidationError(string message)
    {
        AddValidationError(string.Empty, message);
    }

    /// <summary>
    /// Adds a new validation error to the <see cref="ValidationErrors" /> using the specified group.
    /// </summary>
    /// <param name="group">The error group.</param>
    /// <param name="message">The error message.</param>
    public void AddValidationError(string group, string message)
    {
        bool contained = ValidationErrors.TryGetValue(group, out string[]? errors);
        Array.Resize(ref errors, contained ? errors!.Length + 1 : 1);
        errors[^1] = message;
        ValidationErrors[group] = errors;
    }
}
