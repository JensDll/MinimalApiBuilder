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
            .Where(static validator => validator is not null)!
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
        return ValidatorToGenerate.Create(Unsafe.As<ClassDeclarationSyntax>(context.Node),
            context.SemanticModel, cancellationToken);
    }
}
