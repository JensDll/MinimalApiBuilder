﻿namespace MinimalApiBuilder;

public abstract partial class MinimalApiBuilderEndpoint
{
    internal List<string> ValidationErrors { get; } = new();

    protected bool HasValidationError => ValidationErrors.Count > 0;

    protected void AddValidationError(string message)
    {
        ValidationErrors.Add(message);
    }
}
