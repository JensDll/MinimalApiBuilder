using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MinimalApiBuilder.Generator.Providers;

internal static class EndpointProvider
{
    public static IncrementalValuesProvider<ClassDeclarationSyntax> ForEndpointDeclarations(
        this IncrementalGeneratorInitializationContext context)
    {
        return context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsEndpointDeclaration(s),
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
            .Where(static endpointDeclaration => endpointDeclaration is not null)!;
    }

    private static bool IsEndpointDeclaration(SyntaxNode node)
    {
        return node is ClassDeclarationSyntax { BaseList.Types.Count: > 0 } classDeclaration &&
               classDeclaration.BaseList.Types[0].Type is GenericNameSyntax
               {
                   Identifier.Text: "Endpoint",
                   TypeArgumentList.Arguments.Count: 1
               };
    }

    private static ClassDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var endpointDeclaration = (ClassDeclarationSyntax)context.Node;
        var endpointBaseType = endpointDeclaration.BaseList!.Types[0].Type;

        if (context.SemanticModel.GetSymbolInfo(endpointBaseType).Symbol is not INamedTypeSymbol endpointBaseSymbol)
        {
            return null;
        }

        string fullName = endpointBaseSymbol.ToDisplayString();

        return fullName.StartsWith("MinimalApiBuilder.Endpoint") ? endpointDeclaration : null;
    }
}
