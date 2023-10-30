namespace MinimalApiBuilder;

public abstract partial class MinimalApiBuilderEndpoint
{
    protected internal ICollection<string> ValidationErrors { get; } = new List<string>();

    protected bool HasValidationError => ValidationErrors.Count > 0;

    protected void AddValidationError(string message)
    {
        ValidationErrors.Add(message);
    }
}
