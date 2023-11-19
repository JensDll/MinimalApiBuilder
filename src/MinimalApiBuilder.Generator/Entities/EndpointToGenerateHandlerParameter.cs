using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using MinimalApiBuilder.Generator.Common;

namespace MinimalApiBuilder.Generator.Entities;

internal sealed class EndpointToGenerateHandlerParameter : IToGenerate
{
    private readonly string _identifier;

    public EndpointToGenerateHandlerParameter(IParameterSymbol parameter, int position)
    {
        _identifier = parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        Symbol = parameter;
        Position = position;
        IsValueType = parameter.Type.IsValueType;
        HasCustomBinding = false;
        IsNullable = parameter.Type.NullableAnnotation == NullableAnnotation.Annotated;

        ITypeSymbol unwrapped = parameter.Type.UnwrapType();

        foreach (ISymbol member in unwrapped.GetMembers())
        {
            if (member is not IMethodSymbol method)
            {
                continue;
            }

            if (IsBindAsync(method, out var resultType))
            {
                HasCustomBinding = true;
                NeedsCustomBindingNullCheck =
                    !IsNullable && resultType!.TypeArguments[0].NullableAnnotation == NullableAnnotation.Annotated;
                break;
            }

            if (IsTryParse(method, out resultType))
            {
                HasCustomBinding = true;
                NeedsCustomBindingNullCheck =
                    !IsNullable && resultType!.NullableAnnotation == NullableAnnotation.Annotated;
                break;
            }
        }
    }

    public ISymbol Symbol { get; }

    public int Position { get; }

    public bool IsNullable { get; }

    public bool IsValueType { get; }

    public bool HasCustomBinding { get; }

    public bool NeedsCustomBindingNullCheck { get; }

    public override string ToString() => _identifier;

    private static bool IsBindAsync(IMethodSymbol method, out INamedTypeSymbol? resultType)
    {
        switch (method.Parameters.Length)
        {
            case 1:
            {
                resultType = Unsafe.As<INamedTypeSymbol>(method.ReturnType);
                return method is { Name: "BindAsync", DeclaredAccessibility: Accessibility.Public, IsStatic: true } &&
                       method.Parameters[0].Type.IsHttpContext() &&
                       method.ReturnType is INamedTypeSymbol returnType &&
                       returnType.IsValueTask();
            }
            case 2:
            {
                resultType = Unsafe.As<INamedTypeSymbol>(method.ReturnType);
                return method is { Name: "BindAsync", DeclaredAccessibility: Accessibility.Public, IsStatic: true } &&
                       method.Parameters[0].Type.IsHttpContext() &&
                       method.ReturnType is INamedTypeSymbol returnType &&
                       returnType.IsValueTask() &&
                       method.Parameters[1].Type.IsParameterInfo();
            }
            default:
                resultType = null!;
                return false;
        }
    }

    private static bool IsTryParse(IMethodSymbol method, out INamedTypeSymbol? resultType)
    {
        switch (method.Parameters.Length)
        {
            case 2:
            {
                resultType = Unsafe.As<INamedTypeSymbol>(method.Parameters[1].Type);

                return method is
#pragma warning disable IDE0055
#pragma warning restore IDE0055
                       {
                           Name: "TryParse",
                           DeclaredAccessibility: Accessibility.Public,
                           IsStatic: true,
                           ReturnType.SpecialType: SpecialType.System_Boolean
                       }
                       && method.Parameters[0].Type.SpecialType == SpecialType.System_String
                       && method.Parameters[1].RefKind == RefKind.Out;
            }
            case 3:
            {
                resultType = Unsafe.As<INamedTypeSymbol>(method.Parameters[2].Type);

                return method is
#pragma warning disable IDE0055
#pragma warning restore IDE0055
                       {
                           Name: "TryParse",
                           DeclaredAccessibility: Accessibility.Public,
                           IsStatic: true,
                           ReturnType.SpecialType: SpecialType.System_Boolean
                       }
                       && method.Parameters[0].Type.SpecialType == SpecialType.System_String
                       && method.Parameters[1].Type.IsIFormatProvider()
                       && method.Parameters[2].RefKind == RefKind.Out;
            }
            default:
                resultType = null!;
                return false;
        }
    }
}
