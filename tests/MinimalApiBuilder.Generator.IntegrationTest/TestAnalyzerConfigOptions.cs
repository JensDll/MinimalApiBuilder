using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MinimalApiBuilder.Generator.IntegrationTest;

public class TestAnalyzerConfigOptions : AnalyzerConfigOptions
{
    public readonly Dictionary<string, string> Options = new();

    public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
    {
        return Options.TryGetValue(key, out value);
    }
}
