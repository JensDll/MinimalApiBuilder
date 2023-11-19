using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using MinimalApiBuilder.Generator.Common;
using MinimalApiBuilder.Generator.Entities;

namespace MinimalApiBuilder.Generator.Providers;

internal static class ConfigureProvider
{
    public static IncrementalValuesProvider<ConfigureToGenerate> ForConfigure(
        this IncrementalGeneratorInitializationContext context)
    {
        return context.SyntaxProvider
            .CreateSyntaxProvider(IsConfigure, Transform)
            .Where(static value => value is not null)!
            .WithComparer(ConfigureToGenerateEqualityComparer.Instance);
    }

    private static bool IsConfigure(SyntaxNode node, CancellationToken cancellationToken)
    {
        return node is InvocationExpressionSyntax
        {
            Expression: IdentifierNameSyntax { Identifier.ValueText: "Configure" } or
            MemberAccessExpressionSyntax { Name.Identifier.ValueText: "Configure" }
        };
    }

    private static ConfigureToGenerate? Transform(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        if (context.SemanticModel.GetOperation(context.Node, cancellationToken) is not IInvocationOperation configure)
        {
            return null;
        }

        if (configure.IsConfigure(out var builders))
        {
            return ConfigureToGenerate.Create(configure, builders);
        }

        return null;
    }
}
