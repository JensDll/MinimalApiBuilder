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
                transform: static (ctx, _) => (ClassDeclarationSyntax)ctx.Node)
            .Where(static endpointDeclaration => endpointDeclaration is not null)!;
    }

    private static bool IsEndpointDeclaration(SyntaxNode node)
    {
        return node is ClassDeclarationSyntax { BaseList.Types.Count: > 0 } classDeclaration &&
               classDeclaration.BaseList.Types[0].Type is IdentifierNameSyntax
               {
                   Identifier.Text: "MinimalApiBuilderEndpoint"
               };
    }
}
