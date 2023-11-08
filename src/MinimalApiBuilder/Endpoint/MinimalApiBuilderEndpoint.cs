namespace MinimalApiBuilder;

/// <summary>
/// The base class of all minimal API builder endpoints.
/// </summary>
public abstract partial class MinimalApiBuilderEndpoint
{
    /// <summary>
    /// The current validation errors of the request.
    /// </summary>
    protected internal ICollection<string> ValidationErrors { get; } = new List<string>();

    /// <summary>
    /// A <see cref="bool" /> property indicating any validation errors.
    /// </summary>
    public bool HasValidationError => ValidationErrors.Count > 0;

    /// <summary>
    /// Adds a new validation error to the <see cref="ValidationErrors" />.
    /// </summary>
    /// <param name="message">The message of the validation error.</param>
    public void AddValidationError(string message)
    {
        ValidationErrors.Add(message);
    }
}
