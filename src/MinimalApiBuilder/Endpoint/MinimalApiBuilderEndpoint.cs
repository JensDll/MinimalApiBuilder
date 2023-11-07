namespace MinimalApiBuilder;

/// <summary>
/// The base class of all Minimal API Builder endpoints.
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
    /// Add a validation error to <see cref="ValidationErrors" />.
    /// </summary>
    /// <param name="message">The message of the error.</param>
    public void AddValidationError(string message)
    {
        ValidationErrors.Add(message);
    }
}
