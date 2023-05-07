using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MinimalApiBuilder.Generator.Entities;

namespace MinimalApiBuilder.Generator.Providers;

internal static class EndpointProvider
{
    public static IncrementalValuesProvider<EndpointToGenerate> ForEndpoints(
        this IncrementalGeneratorInitializationContext context)
    {
        return context.SyntaxProvider
            .CreateSyntaxProvider(IsEndpointDeclaration, Transform)
            .Where(static endpoint => endpoint is not null)!
            .WithComparer(EndpointToGenerateEqualityComparer.Instance);
    }

    private static bool IsEndpointDeclaration(SyntaxNode node, CancellationToken cancellationToken)
    {
        return node is ClassDeclarationSyntax { BaseList.Types.Count: > 0 } classDeclaration &&
               classDeclaration.BaseList.Types[0].Type is IdentifierNameSyntax
               {
                   Identifier.Text: "MinimalApiBuilderEndpoint"
               };
    }

    private static EndpointToGenerate? Transform(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        return EndpointToGenerate.Create(Unsafe.As<ClassDeclarationSyntax>(context.Node),
            context.SemanticModel, cancellationToken);
    }
}
