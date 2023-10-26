using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MinimalApiBuilder.Generator.Common;

namespace MinimalApiBuilder.Generator.Entities;

internal class ValidatorToGenerate
{
    private const string AbstractValidatorName = "FluentValidation.AbstractValidator`1";
    private const string RegisterValidatorAttributeName = "MinimalApiBuilder.RegisterValidatorAttribute";

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

        if (semanticModel.Compilation.GetTypeByMetadataName(AbstractValidatorName)
            is not { } abstractValidatorSymbol)
        {
            return null;
        }

        if (semanticModel.Compilation.GetTypeByMetadataName(RegisterValidatorAttributeName)
            is not { } registerValidatorAttributeSymbol)
        {
            return null;
        }

        if (!abstractValidatorSymbol.Equals(validatorSymbol.BaseType!.OriginalDefinition,
                SymbolEqualityComparer.Default))
        {
            return null;
        }

        bool isAsync = GetIsAsync(validatorDeclaration, cancellationToken);
        string serviceLifetime = GetValidatorServiceLifetime(validatorSymbol, registerValidatorAttributeSymbol);

        ValidatorToGenerate validator = new(
            identifier: validatorSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            validatedType: validatorSymbol.BaseType!.TypeArguments[0]
                .ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            isAsync: isAsync,
            serviceLifetime: serviceLifetime);

        return validator;
    }

    private static string GetValidatorServiceLifetime(ISymbol validatorSymbol,
        ISymbol registerValidatorAttributeSymbol)
    {
        foreach (AttributeData attribute in validatorSymbol.GetAttributes())
        {
            if (!registerValidatorAttributeSymbol.Equals(attribute.AttributeClass, SymbolEqualityComparer.Default) ||
                attribute.ConstructorArguments.Length != 1 ||
                attribute.ConstructorArguments[0].Value is not int lifetime)
            {
                continue;
            }

            return lifetime.ServiceLifetimeToString();
        }

        return "Singleton";
    }

    private static bool GetIsAsync(TypeDeclarationSyntax validatorDeclaration, CancellationToken cancellationToken)
    {
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
                    return true;
                }
            }
        }

        return false;
    }
}
