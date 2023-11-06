using Microsoft.CodeAnalysis;

namespace MinimalApiBuilder.Generator.Common;

internal static class Extensions
{
    public static string ServiceLifetimeToString(this int value)
    {
        return value switch
        {
            0 => "Singleton",
            1 => "Scoped",
            2 => "Transient",
            _ => "Singleton"
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
