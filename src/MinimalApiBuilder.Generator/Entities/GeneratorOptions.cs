using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using MinimalApiBuilder.Generator.Common;

namespace MinimalApiBuilder.Generator.Entities;

internal readonly struct GeneratorOptions
{
    private readonly AnalyzerConfigOptionsProvider _provider;

    public bool AssignNameToEndpoint { get; } = false;

    public GeneratorOptions(AnalyzerConfigOptionsProvider optionsProvider)
    {
        _provider = optionsProvider;

        AnalyzerConfigOptions options = optionsProvider.GlobalOptions;

        if (options.TryGetValue(Keys.AssignNameToEndpointBuildProperty, out string? assignNameToEndpoint))
        {
            AssignNameToEndpoint = assignNameToEndpoint.Equals("true", StringComparison.OrdinalIgnoreCase);
        }
    }

    private GeneratorOptions(GeneratorOptions other, SyntaxTree syntaxTree)
    {
        _provider = other._provider;
        AssignNameToEndpoint = other.AssignNameToEndpoint;

        AnalyzerConfigOptions options = _provider.GetOptions(syntaxTree);

        if (options.TryGetValue(Keys.AssignNameToEndpoint, out string? assignNameToEndpoint))
        {
            AssignNameToEndpoint = assignNameToEndpoint.Equals("true", StringComparison.OrdinalIgnoreCase);
        }
    }

    public GeneratorOptions GetForTarget(IWithSyntaxTree target)
    {
        return new GeneratorOptions(this, target.SyntaxTree);
    }

    internal static class Keys
    {
        private const string Prefix = "minimalapibuilder_";
        private const string BuildProperty = "build_property.";

        public const string AssignNameToEndpoint = Prefix + "assign_name_to_endpoint";
        public const string AssignNameToEndpointBuildProperty = BuildProperty + AssignNameToEndpoint;
    }
}
