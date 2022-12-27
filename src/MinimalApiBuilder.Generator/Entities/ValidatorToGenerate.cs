using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MinimalApiBuilder.Generator.Entities;

internal class ValidatorToGenerate
{
    private readonly string _identifier;

    private ValidatorToGenerate(string identifier, bool isAsync)
    {
        _identifier = identifier;
        IsAsync = isAsync;
    }

    public bool IsAsync { get; }

    public override string ToString() => _identifier;

    public static IReadOnlyDictionary<string, ValidatorToGenerate> Collect(Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax> validatorDeclarations,
        CancellationToken cancellationToken)
    {
        Dictionary<string, ValidatorToGenerate> validators = new();

        foreach (ClassDeclarationSyntax validatorDeclaration in validatorDeclarations)
        {
            cancellationToken.ThrowIfCancellationRequested();

            SemanticModel semanticModel = compilation.GetSemanticModel(validatorDeclaration.SyntaxTree);

            if (semanticModel.GetDeclaredSymbol(validatorDeclaration) is not INamedTypeSymbol validatorSymbol)
            {
                continue;
            }

            bool isAsync = false;

            foreach (MemberDeclarationSyntax member in validatorDeclaration.Members)
            {
                if (member is not ConstructorDeclarationSyntax constructorDeclaration)
                {
                    continue;
                }

                foreach (SyntaxNode node in constructorDeclaration.DescendantNodes())
                {
                    if (node is not IdentifierNameSyntax validatorNode ||
                        node.Parent is not MemberAccessExpressionSyntax ||
                        node.Parent.Parent is not InvocationExpressionSyntax ||
                        validatorNode.Identifier.Text is not ("MustAsync" or "WhenAsync" or "CustomAsync"))
                    {
                        continue;
                    }

                    isAsync = true;
                    break;
                }
            }

            ValidatorToGenerate validator = new(
                identifier: validatorSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                isAsync: isAsync);

            validators.Add(
                validatorSymbol.BaseType!.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                validator);
        }

        return validators;
    }
}
