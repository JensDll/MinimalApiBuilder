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
            .Where(static value => value is not null)!
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
        if (context.SemanticModel.GetDeclaredSymbol(context.Node, cancellationToken) is not INamedTypeSymbol endpoint)
        {
            return null;
        }

        ClassDeclarationSyntax endpointSyntax = Unsafe.As<ClassDeclarationSyntax>(context.Node);

        return EndpointToGenerate.Create(endpoint, endpointSyntax, cancellationToken);
    }
}
