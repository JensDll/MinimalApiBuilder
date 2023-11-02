using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MinimalApiBuilder.Generator.Common;

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

    public static ValidatorToGenerate? Create(
        INamedTypeSymbol validator,
        ClassDeclarationSyntax validatorSyntax,
        WellKnownTypes wellKnownTypes,
        CancellationToken cancellationToken)
    {
        INamedTypeSymbol abstractValidator = wellKnownTypes[WellKnownTypes.Type.FluentValidation_AbstractValidator_1];

        if (!SymbolEqualityComparer.Default.Equals(validator.BaseType?.OriginalDefinition, abstractValidator))
        {
            return null;
        }

        INamedTypeSymbol registerValidatorAttribute =
            wellKnownTypes[WellKnownTypes.Type.MinimalApiBuilder_RegisterValidatorAttribute];

        bool isAsync = GetIsAsync(validatorSyntax);
        string serviceLifetime = GetValidatorServiceLifetime(validator, registerValidatorAttribute);

        cancellationToken.ThrowIfCancellationRequested();

        ValidatorToGenerate result = new(
            identifier: validator.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            validatedType: validator.BaseType.TypeArguments[0]
                .ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            isAsync: isAsync,
            serviceLifetime: serviceLifetime);

        return result;
    }

    private static string GetValidatorServiceLifetime(ISymbol validator, ISymbol registerValidatorAttribute)
    {
        foreach (AttributeData attribute in validator.GetAttributes())
        {
            if (!SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, registerValidatorAttribute) ||
                attribute.ConstructorArguments.Length != 1 ||
                attribute.ConstructorArguments[0].Value is not int lifetime)
            {
                continue;
            }

            return lifetime.ServiceLifetimeToString();
        }

        return "Singleton";
    }

    private static bool GetIsAsync(TypeDeclarationSyntax validatorSyntax)
    {
        foreach (MemberDeclarationSyntax member in validatorSyntax.Members)
        {
            if (member is not ConstructorDeclarationSyntax constructorSyntax)
            {
                continue;
            }

            return constructorSyntax.DescendantNodes()
                .OfType<IdentifierNameSyntax>()
                .Any(static nameSyntax =>
                    nameSyntax.Parent is MemberAccessExpressionSyntax &&
                    nameSyntax.Parent.Parent is InvocationExpressionSyntax &&
                    nameSyntax.Identifier.Text is "MustAsync" or "WhenAsync" or "UnlessAsync" or "CustomAsync"
                        or "SetAsyncValidator");
        }

        return false;
    }
}
