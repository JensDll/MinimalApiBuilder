using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
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

        if (configure.TargetMethod.ContainingNamespace is { Name: "MinimalApiBuilder", ContainingNamespace.IsGlobalNamespace: true } &&
            configure.Arguments.Length == 1 &&
            configure.Arguments[0].Value is IArrayCreationOperation { Initializer: IArrayInitializerOperation builders })
        {
            return ConfigureToGenerate.Create(configure, builders);
        }

        return null;
    }
}
