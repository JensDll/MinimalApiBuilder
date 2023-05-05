using Microsoft.CodeAnalysis;

namespace MinimalApiBuilder.Generator.Providers;

public static class OptionsProvider
{
    public static void ForOptions(this IncrementalGeneratorInitializationContext context)
    {
        var result = context.AnalyzerConfigOptionsProvider.Select(static (provider, _) =>
        {
            provider.GlobalOptions.TryGetValue("build_property.MinimalApiBuilder_AssignNameToEndpoint",
                out string? value);
            return value;
        });
    }
}
