using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MinimalApiBuilder.Generator.Entities;

internal class ValidatorToGenerate
{
    private readonly string _identifier;

    private ValidatorToGenerate(
        string identifier,
        string validatedType,
        bool isAsync,
        string serviceLifetime)
    {
        _identifier = identifier;
        ValidatedType = validatedType;
        IsAsync = isAsync;
        ServiceLifetime = serviceLifetime;
    }

    public string ValidatedType { get; }

    public bool IsAsync { get; }

    public string ServiceLifetime { get; }

    public override string ToString() => _identifier;

    public static ValidatorToGenerate? Create(ClassDeclarationSyntax validatorDeclaration,
        SemanticModel semanticModel, CancellationToken cancellationToken)
    {
        if (semanticModel.GetDeclaredSymbol(validatorDeclaration, cancellationToken)
            is not INamedTypeSymbol validatorSymbol)
        {
            return null;
        }

        if (semanticModel.Compilation.GetTypeByMetadataName("FluentValidation.AbstractValidator`1")
            is not { } abstractValidatorSymbol)
        {
            return null;
        }

        if (!abstractValidatorSymbol.Equals(validatorSymbol.BaseType!.OriginalDefinition,
                SymbolEqualityComparer.Default))
        {
            return null;
        }

        bool isAsync = false;

        foreach (MemberDeclarationSyntax member in validatorDeclaration.Members)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (member is not ConstructorDeclarationSyntax constructorDeclaration)
            {
                continue;
            }

            foreach (SyntaxNode node in constructorDeclaration.DescendantNodes())
            {
                if (node is IdentifierNameSyntax validatorNode &&
                    node.Parent is MemberAccessExpressionSyntax &&
                    node.Parent.Parent is InvocationExpressionSyntax &&
                    validatorNode.Identifier.Text is "MustAsync" or "WhenAsync" or "UnlessAsync"
                        or "CustomAsync" or "SetAsyncValidator")
                {
                    isAsync = true;
                    goto MembersParsed;
                }
            }
        }

    MembersParsed:

        string serviceLifetime = GetValidatorServiceLifetime(validatorDeclaration);

        ValidatorToGenerate validator = new(
            identifier: validatorSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            validatedType: validatorSymbol.BaseType!.TypeArguments[0]
                .ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            isAsync: isAsync,
            serviceLifetime: serviceLifetime);

        return validator;
    }

    private static string GetValidatorServiceLifetime(MemberDeclarationSyntax validatorDeclaration)
    {
        foreach (AttributeListSyntax attribute in validatorDeclaration.AttributeLists)
        {
            string name = attribute.ToString();

            const string pattern = @"RegisterValidator\(ServiceLifetime\.(?<lifetime>\w+)";
            Match match = Regex.Match(name, pattern);

            if (match.Success)
            {
                return match.Groups["lifetime"].Value;
            }
        }

        return "Singleton";
    }
}

internal class ValidatorToGenerateEqualityComparer : IEqualityComparer<ValidatorToGenerate>
{
    public static readonly ValidatorToGenerateEqualityComparer Instance = new();

    public bool Equals(ValidatorToGenerate? x, ValidatorToGenerate? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null)
        {
            return false;
        }

        if (y is null)
        {
            return false;
        }

        return x.ValidatedType == y.ValidatedType &&
               x.IsAsync == y.IsAsync &&
               x.ServiceLifetime == y.ServiceLifetime;
    }

    public int GetHashCode(ValidatorToGenerate obj)
    {
        throw new NotImplementedException();
    }
}
