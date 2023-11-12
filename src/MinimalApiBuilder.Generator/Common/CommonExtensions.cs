using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace MinimalApiBuilder.Generator.Common;

internal static class CommonExtensions
{
    public static string ToStringEnum(this ServiceLifetime value)
    {
        return value switch
        {
            ServiceLifetime.Singleton => "Singleton",
            ServiceLifetime.Scoped => "Scoped",
            ServiceLifetime.Transient => "Transient",
            _ => "Singleton"
        };
    }

    public static string ToAccessibilityString(this Accessibility accessibility)
    {
        return accessibility switch
        {
            Accessibility.Private => "private",
            Accessibility.ProtectedAndInternal => "protected internal",
            Accessibility.Protected => "protected",
            Accessibility.Internal => "internal",
            Accessibility.ProtectedOrInternal => "protected internal",
            _ => "public"
        };
    }

    public static ITypeSymbol UnwrapType(this ITypeSymbol type)
    {
        INamedTypeSymbol? unwrapped = type switch
        {
            IArrayTypeSymbol arrayType => arrayType.ElementType as INamedTypeSymbol,
            INamedTypeSymbol namedType => namedType,
            _ => null
        };

        unwrapped = unwrapped?.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T
            ? unwrapped.TypeArguments[0] as INamedTypeSymbol
            : unwrapped;

        return unwrapped ?? type;
    }
}
