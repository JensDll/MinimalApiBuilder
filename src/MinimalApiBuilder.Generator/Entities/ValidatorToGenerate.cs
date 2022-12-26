using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MinimalApiBuilder.Generator.Entities;

internal class ValidatorToGenerate
{
    private ValidatorToGenerate(string id)
    {
        Id = id;
    }

    public string Id { get; }

    public static IEnumerable<ValidatorToGenerate> Collect(Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax> validatorDeclarations,
        CancellationToken cancellationToken)
    {
        foreach (ClassDeclarationSyntax validatorDeclaration in validatorDeclarations)
        {
            cancellationToken.ThrowIfCancellationRequested();

            SemanticModel semanticModel = compilation.GetSemanticModel(validatorDeclaration.SyntaxTree);

            if (semanticModel.GetDeclaredSymbol(validatorDeclaration) is not INamedTypeSymbol validatorSymbol)
            {
                continue;
            }

            yield return new ValidatorToGenerate(
                validatorSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
        }
    }
}
