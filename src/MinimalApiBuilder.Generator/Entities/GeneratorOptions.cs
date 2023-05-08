using Microsoft.CodeAnalysis.Diagnostics;

namespace MinimalApiBuilder.Generator.Entities;

internal struct GeneratorOptions
{
    public bool AssignNameToEndpoint { get; }

    public GeneratorOptions(AnalyzerConfigOptionsProvider optionsProvider)
    {
        if (optionsProvider.GlobalOptions.TryGetValue("build_property." + Keys.AssignNameToEndpoint,
                out string? assignNameToEndpoint) ||
            optionsProvider.GlobalOptions.TryGetValue(Keys.AssignNameToEndpoint, out assignNameToEndpoint))
        {
            AssignNameToEndpoint = assignNameToEndpoint.Equals("true", StringComparison.OrdinalIgnoreCase);
        }
    }

    internal static class Keys
    {
        private const string Prefix = "minimalapibuilder_";

        public const string AssignNameToEndpoint = Prefix + "assign_name_to_endpoint";
    }
}
