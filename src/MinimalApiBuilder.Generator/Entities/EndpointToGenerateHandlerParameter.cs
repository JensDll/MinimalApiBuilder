using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using MinimalApiBuilder.Generator.Common;

namespace MinimalApiBuilder.Generator.Entities;

internal class EndpointToGenerateHandlerParameter
{
    private readonly string _identifier;

    public EndpointToGenerateHandlerParameter(IParameterSymbol parameter, int position, WellKnownTypes wellKnownTypes)
    {
        _identifier = parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        Position = position;
        IsValueType = parameter.Type.IsValueType;
        HasCustomBinding = false;
        NeedsNullValidation = parameter.Type.NullableAnnotation == NullableAnnotation.Annotated;

        ITypeSymbol unwrapped = parameter.Type.UnwrapType();

        foreach (ISymbol member in unwrapped.GetMembers())
        {
            if (member is not IMethodSymbol method)
            {
                continue;
            }

            if (IsBindAsync(method, wellKnownTypes, out INamedTypeSymbol? resultType))
            {
                HasCustomBinding = true;
                NeedsNullValidation = resultType!.TypeArguments[0].NullableAnnotation == NullableAnnotation.Annotated;
                break;
            }

            if (IsTryParse(method, wellKnownTypes, out resultType))
            {
                HasCustomBinding = true;
                NeedsNullValidation = resultType!.NullableAnnotation == NullableAnnotation.Annotated;
                break;
            }
        }
    }

    public int Position { get; }

    public bool NeedsNullValidation { get; }

    public bool IsValueType { get; }

    public bool HasCustomBinding { get; }

    public override string ToString() => _identifier;

    private static bool IsBindAsync(IMethodSymbol method, WellKnownTypes wellKnownTypes,
        out INamedTypeSymbol? resultType)
    {
        INamedTypeSymbol httpContext = wellKnownTypes[WellKnownTypes.Type.Microsoft_AspNetCore_Http_HttpContext];
        INamedTypeSymbol valueTask = wellKnownTypes[WellKnownTypes.Type.System_Threading_Tasks_ValueTask_1];

        switch (method.Parameters.Length)
        {
            case 1:
            {
                resultType = Unsafe.As<INamedTypeSymbol>(method.ReturnType);
                return method is { Name: "BindAsync", DeclaredAccessibility: Accessibility.Public, IsStatic: true } &&
                       SymbolEqualityComparer.Default.Equals(method.Parameters[0].Type, httpContext) &&
                       method.ReturnType is INamedTypeSymbol returnType &&
                       SymbolEqualityComparer.Default.Equals(returnType.ConstructedFrom, valueTask);
            }
            case 2:
            {
                INamedTypeSymbol parameterInfo = wellKnownTypes[WellKnownTypes.Type.System_Reflection_ParameterInfo];
                resultType = Unsafe.As<INamedTypeSymbol>(method.ReturnType);
                return method is { Name: "BindAsync", DeclaredAccessibility: Accessibility.Public, IsStatic: true } &&
                       SymbolEqualityComparer.Default.Equals(method.Parameters[0].Type, httpContext) &&
                       method.ReturnType is INamedTypeSymbol returnType &&
                       SymbolEqualityComparer.Default.Equals(returnType.ConstructedFrom, valueTask) &&
                       SymbolEqualityComparer.Default.Equals(method.Parameters[1].Type, parameterInfo);
            }
            default:
                resultType = null!;
                return false;
        }
    }

    private static bool IsTryParse(IMethodSymbol method, WellKnownTypes wellKnownTypes,
        out INamedTypeSymbol? resultType)
    {
        switch (method.Parameters.Length)
        {
            case 2:
            {
                resultType = Unsafe.As<INamedTypeSymbol>(method.Parameters[1].Type);
#pragma warning disable IDE0055
                return method is
                       {
                           Name: "TryParse",
                           DeclaredAccessibility: Accessibility.Public,
                           IsStatic: true,
                           ReturnType.SpecialType: SpecialType.System_Boolean
                       }
#pragma warning restore IDE0055
                       && method.Parameters[0].Type.SpecialType == SpecialType.System_String
                       && method.Parameters[1].RefKind == RefKind.Out;
            }
            case 3:
            {
                INamedTypeSymbol formatProvider = wellKnownTypes[WellKnownTypes.Type.System_IFormatProvider];
                resultType = Unsafe.As<INamedTypeSymbol>(method.Parameters[2].Type);
#pragma warning disable IDE0055
                return method is
                       {
                           Name: "TryParse",
                           DeclaredAccessibility: Accessibility.Public,
                           IsStatic: true,
                           ReturnType.SpecialType: SpecialType.System_Boolean
                       }
#pragma warning restore IDE0055
                       && method.Parameters[0].Type.SpecialType == SpecialType.System_String
                       && SymbolEqualityComparer.Default.Equals(method.Parameters[1].Type, formatProvider)
                       && method.Parameters[2].RefKind == RefKind.Out;
            }
            default:
                resultType = null!;
                return false;
        }
    }
}
