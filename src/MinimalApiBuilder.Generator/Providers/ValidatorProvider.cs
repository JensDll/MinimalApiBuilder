using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MinimalApiBuilder.Generator.Providers;

internal static class ValidatorProvider
{
    public static IncrementalValuesProvider<ClassDeclarationSyntax> ForValidatorDeclarations(
        this IncrementalGeneratorInitializationContext context)
    {
        return context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsValidator(s),
                transform: static (ctx, _) => (ClassDeclarationSyntax)ctx.Node);
    }

    private static bool IsValidator(SyntaxNode node)
    {
        return node is ClassDeclarationSyntax { BaseList.Types.Count: > 0 } classDeclaration &&
               classDeclaration.BaseList.Types[0].Type is GenericNameSyntax
               {
                   Identifier.Text: "AbstractValidator",
                   TypeArgumentList.Arguments.Count: 1
               };
    }
}
