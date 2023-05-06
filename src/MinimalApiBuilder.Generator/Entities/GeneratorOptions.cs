using Microsoft.CodeAnalysis.Diagnostics;

namespace MinimalApiBuilder.Generator.Entities;

internal class GeneratorOptions
{
    public bool AssignNameToEndpoint { get; } = true;

    public GeneratorOptions(AnalyzerConfigOptionsProvider optionsProvider)
    {
        if (optionsProvider.GlobalOptions.TryGetValue("build_property." + Keys.AssignNameToEndpoint,
                out string? assignNameToEndpoint))
        {
            AssignNameToEndpoint = assignNameToEndpoint.Equals("true", StringComparison.OrdinalIgnoreCase);
        }
    }

    internal class Keys
    {
        private const string Prefix = "minimalapibuilder_";

        public const string AssignNameToEndpoint = Prefix + "assign_name_to_endpoint";
    }
}
