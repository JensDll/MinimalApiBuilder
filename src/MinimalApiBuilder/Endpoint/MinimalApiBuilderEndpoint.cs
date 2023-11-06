namespace MinimalApiBuilder;

public abstract partial class MinimalApiBuilderEndpoint
{
    protected internal ICollection<string> ValidationErrors { get; } = new List<string>();

    public bool HasValidationError => ValidationErrors.Count > 0;

    public void AddValidationError(string message)
    {
        ValidationErrors.Add(message);
    }
}
