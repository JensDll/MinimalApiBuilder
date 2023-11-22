using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using MinimalApiBuilder.Generator.Common;

namespace MinimalApiBuilder.Generator.Entities;

internal readonly struct GeneratorOptions
{
    private readonly AnalyzerConfigOptionsProvider _provider;

    public bool AssignNameToEndpoint { get; } = false;

    public string ValidationProblemTitle { get; } = "One or more validation errors occurred.";

    public string ModelBindingProblemTitle { get; } = "One or more model binding errors occurred.";

    public GeneratorOptions(AnalyzerConfigOptionsProvider optionsProvider)
    {
        _provider = optionsProvider;

        AnalyzerConfigOptions options = optionsProvider.GlobalOptions;

        if (options.TryGetValue(Keys.AssignNameToEndpointBuildProperty, out string? assignNameToEndpoint))
        {
            AssignNameToEndpoint = assignNameToEndpoint.Equals("true", StringComparison.OrdinalIgnoreCase);
        }

        if (options.TryGetValue(Keys.ValidationProblemTitleBuildProperty, out string? validationProblemTitle))
        {
            ValidationProblemTitle = validationProblemTitle;
        }

        if (options.TryGetValue(Keys.ModelBindingProblemTitleBuildProperty, out string? modelBindingProblemTitle))
        {
            ModelBindingProblemTitle = modelBindingProblemTitle;
        }
    }

    private GeneratorOptions(GeneratorOptions other, SyntaxTree syntaxTree)
    {
        _provider = other._provider;
        AssignNameToEndpoint = other.AssignNameToEndpoint;
        ValidationProblemTitle = other.ValidationProblemTitle;
        ModelBindingProblemTitle = other.ModelBindingProblemTitle;

        AnalyzerConfigOptions options = _provider.GetOptions(syntaxTree);

        if (options.TryGetValue(Keys.AssignNameToEndpoint, out string? assignNameToEndpoint))
        {
            AssignNameToEndpoint = assignNameToEndpoint.Equals("true", StringComparison.OrdinalIgnoreCase);
        }

        if (options.TryGetValue(Keys.ValidationProblemTitle, out string? validationProblemTitle))
        {
            ValidationProblemTitle = validationProblemTitle;
        }

        if (options.TryGetValue(Keys.ModelBindingProblemTitle, out string? modelBindingProblemTitle))
        {
            ModelBindingProblemTitle = modelBindingProblemTitle;
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

        public const string ValidationProblemTitle = Prefix + "validation_problem_title";
        public const string ValidationProblemTitleBuildProperty = BuildProperty + ValidationProblemTitle;

        public const string ModelBindingProblemTitle = Prefix + "model_binding_problem_title";
        public const string ModelBindingProblemTitleBuildProperty = BuildProperty + ModelBindingProblemTitle;
    }
}
