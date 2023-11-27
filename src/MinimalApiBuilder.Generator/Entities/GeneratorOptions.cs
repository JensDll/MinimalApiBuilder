using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using MinimalApiBuilder.Generator.Common;

namespace MinimalApiBuilder.Generator.Entities;

internal readonly struct GeneratorOptions
{
    private const string DefaultProblemType = "https://tools.ietf.org/html/rfc9110#section-15.5.1";

    private readonly AnalyzerConfigOptionsProvider _provider;

    public bool AssignNameToEndpoint { get; } = false;

    public string ValidationProblemType { get; } = DefaultProblemType;

    public string ValidationProblemTitle { get; } = "One or more validation errors occurred.";

    public string ModelBindingProblemType { get; } = DefaultProblemType;

    public string ModelBindingProblemTitle { get; } = "One or more model binding errors occurred.";

    public GeneratorOptions(AnalyzerConfigOptionsProvider optionsProvider)
    {
        _provider = optionsProvider;

        AnalyzerConfigOptions options = optionsProvider.GlobalOptions;

        if (options.TryGetValue(Keys.AssignNameToEndpointBuildProperty, out string? assignNameToEndpoint)
            && !string.IsNullOrEmpty(assignNameToEndpoint))
        {
            AssignNameToEndpoint = assignNameToEndpoint.Equals("true", StringComparison.OrdinalIgnoreCase);
        }

        if (options.TryGetValue(Keys.ValidationProblemTypeBuildProperty, out string? validationProblemType)
            && !string.IsNullOrEmpty(validationProblemType))
        {
            ValidationProblemType = validationProblemType;
        }

        if (options.TryGetValue(Keys.ValidationProblemTitleBuildProperty, out string? validationProblemTitle)
            && !string.IsNullOrEmpty(validationProblemTitle))
        {
            ValidationProblemTitle = validationProblemTitle;
        }

        if (options.TryGetValue(Keys.ModelBindingProblemTypeBuildProperty, out string? modelBindingProblemType)
            && !string.IsNullOrEmpty(modelBindingProblemType))
        {
            ModelBindingProblemType = modelBindingProblemType;
        }

        if (options.TryGetValue(Keys.ModelBindingProblemTitleBuildProperty, out string? modelBindingProblemTitle)
            && !string.IsNullOrEmpty(modelBindingProblemTitle))
        {
            ModelBindingProblemTitle = modelBindingProblemTitle;
        }
    }

    private GeneratorOptions(GeneratorOptions other, SyntaxTree syntaxTree)
    {
        _provider = other._provider;
        AssignNameToEndpoint = other.AssignNameToEndpoint;
        ValidationProblemType = other.ValidationProblemType;
        ValidationProblemTitle = other.ValidationProblemTitle;
        ModelBindingProblemType = other.ModelBindingProblemType;
        ModelBindingProblemTitle = other.ModelBindingProblemTitle;

        AnalyzerConfigOptions options = _provider.GetOptions(syntaxTree);

        if (options.TryGetValue(Keys.AssignNameToEndpoint, out string? assignNameToEndpoint)
            && !string.IsNullOrEmpty(assignNameToEndpoint))
        {
            AssignNameToEndpoint = assignNameToEndpoint.Equals("true", StringComparison.OrdinalIgnoreCase);
        }

        if (options.TryGetValue(Keys.ValidationProblemType, out string? validationProblemType)
            && !string.IsNullOrEmpty(validationProblemType))
        {
            ValidationProblemType = validationProblemType;
        }

        if (options.TryGetValue(Keys.ValidationProblemTitle, out string? validationProblemTitle)
            && !string.IsNullOrEmpty(validationProblemTitle))
        {
            ValidationProblemTitle = validationProblemTitle;
        }

        if (options.TryGetValue(Keys.ModelBindingProblemType, out string? modelBindingProblemType)
            && !string.IsNullOrEmpty(modelBindingProblemType))
        {
            ModelBindingProblemType = modelBindingProblemType;
        }

        if (options.TryGetValue(Keys.ModelBindingProblemTitle, out string? modelBindingProblemTitle)
            && !string.IsNullOrEmpty(modelBindingProblemTitle))
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

        public const string ValidationProblemType = Prefix + "validation_problem_type";
        public const string ValidationProblemTypeBuildProperty = BuildProperty + ValidationProblemType;

        public const string ModelBindingProblemTitle = Prefix + "model_binding_problem_title";
        public const string ModelBindingProblemTitleBuildProperty = BuildProperty + ModelBindingProblemTitle;

        public const string ModelBindingProblemType = Prefix + "model_binding_problem_type";
        public const string ModelBindingProblemTypeBuildProperty = BuildProperty + ModelBindingProblemType;
    }
}
