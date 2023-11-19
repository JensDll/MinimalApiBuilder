using Microsoft.CodeAnalysis;

namespace MinimalApiBuilder.Generator.Common;

internal static class CommonExtensions
{
    public static string ToStringServiceLifetime(this int value)
    {
        return value switch
        {
            0 => "Singleton",
            1 => "Scoped",
            2 => "Transient",
            _ => "Singleton"
        };
    }

    public static string ToStringEnum(this Accessibility accessibility)
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

    public static string GetInterceptorFilePath(this SyntaxTree tree, SourceReferenceResolver? resolver)
    {
        return resolver?.NormalizePath(tree.FilePath, baseFilePath: null) ?? tree.FilePath;
    }
}
