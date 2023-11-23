using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MinimalApiBuilder.Generator.Common;

namespace MinimalApiBuilder.Generator.Entities;

internal sealed class ValidatorToGenerate
{
    private readonly string _identifier;

    private ValidatorToGenerate(
        ITypeSymbol validator,
        bool isAsync,
        int serviceLifetime)
    {
        _identifier = validator.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        ValidatedType = validator.BaseType!.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        IsAsync = isAsync;
        ServiceLifetime = serviceLifetime;
    }

    public string ValidatedType { get; }

    public bool IsAsync { get; }

    public int ServiceLifetime { get; }

    public override string ToString() => _identifier;

    public static ValidatorToGenerate? Create(
        INamedTypeSymbol validator,
        ClassDeclarationSyntax validatorSyntax,
        CancellationToken cancellationToken)
    {
        if (validator.BaseType is null ||
            !validator.BaseType.IsAbstractValidator())
        {
            return null;
        }

        bool isAsync = GetIsAsync(validatorSyntax);
        int serviceLifetime = GetValidatorServiceLifetime(validator);

        cancellationToken.ThrowIfCancellationRequested();

        ValidatorToGenerate result = new(
            validator: validator,
            isAsync: isAsync,
            serviceLifetime: serviceLifetime);

        return result;
    }

    private static int GetValidatorServiceLifetime(ISymbol validator)
    {
        foreach (AttributeData attribute in validator.GetAttributes())
        {
            if (attribute.AttributeClass is not null &&
                attribute.AttributeClass.IsRegisterValidatorAttribute() &&
                attribute.ConstructorArguments.Length == 1 &&
                attribute.ConstructorArguments[0].Value is int lifetime)
            {
                return lifetime;
            }
        }

        return 0;
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
