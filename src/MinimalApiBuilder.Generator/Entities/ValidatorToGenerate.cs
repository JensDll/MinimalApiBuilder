using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MinimalApiBuilder.Generator.Common;

namespace MinimalApiBuilder.Generator.Entities;

internal class ValidatorToGenerate : IToGenerate
{
    private readonly string _identifier;

    private ValidatorToGenerate(
        ITypeSymbol validator,
        bool isAsync,
        string serviceLifetime)
    {
        _identifier = validator.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        Symbol = validator;
        ValidatedType = validator.BaseType!.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        IsAsync = isAsync;
        ServiceLifetime = serviceLifetime;
    }

    public ISymbol Symbol { get; }

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

        bool isAsync = GetIsAsync(validatorSyntax);
        string serviceLifetime = GetValidatorServiceLifetime(validator, wellKnownTypes);

        cancellationToken.ThrowIfCancellationRequested();

        ValidatorToGenerate result = new(
            validator: validator,
            isAsync: isAsync,
            serviceLifetime: serviceLifetime);

        return result;
    }

    private static string GetValidatorServiceLifetime(ISymbol validator, WellKnownTypes wellKnownTypes)
    {
        INamedTypeSymbol registerValidatorAttribute =
            wellKnownTypes[WellKnownTypes.Type.MinimalApiBuilder_RegisterValidatorAttribute];

        foreach (AttributeData attribute in validator.GetAttributes())
        {
            if (!SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, registerValidatorAttribute) ||
                attribute.ConstructorArguments.Length != 1 ||
                attribute.ConstructorArguments[0].Value is not int lifetime)
            {
                continue;
            }

            return lifetime.ToServiceLifetimeString();
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
