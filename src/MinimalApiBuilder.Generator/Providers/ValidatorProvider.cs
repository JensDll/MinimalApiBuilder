using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MinimalApiBuilder.Generator.Entities;

namespace MinimalApiBuilder.Generator.Providers;

internal static class ValidatorProvider
{
    public static IncrementalValuesProvider<ValidatorToGenerate> ForValidators(
        this IncrementalGeneratorInitializationContext context)
    {
        return context.SyntaxProvider
            .CreateSyntaxProvider(IsValidator, Transform)
            .Where(static value => value is not null)!
            .WithComparer(ValidatorToGenerateEqualityComparer.Instance);
    }

    private static bool IsValidator(SyntaxNode node, CancellationToken cancellationToken)
    {
        return node is ClassDeclarationSyntax { BaseList.Types.Count: > 0 } classDeclaration &&
               classDeclaration.BaseList.Types[0].Type is GenericNameSyntax
               {
                   Identifier.Text: "AbstractValidator",
                   TypeArgumentList.Arguments.Count: 1
               };
    }

    private static ValidatorToGenerate? Transform(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        if (context.SemanticModel.GetDeclaredSymbol(context.Node, cancellationToken) is not INamedTypeSymbol validator)
        {
            return null;
        }

        ClassDeclarationSyntax validatorSyntax = Unsafe.As<ClassDeclarationSyntax>(context.Node);

        return ValidatorToGenerate.Create(validator, validatorSyntax, cancellationToken);
    }
}
