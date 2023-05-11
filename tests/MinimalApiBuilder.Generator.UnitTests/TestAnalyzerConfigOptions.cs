using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MinimalApiBuilder.Generator.UnitTests;

public class TestAnalyzerConfigOptions : AnalyzerConfigOptions
{
    public Dictionary<string, string> Options { get; } = new();

    public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
    {
        return Options.TryGetValue(key, out value);
    }
}
