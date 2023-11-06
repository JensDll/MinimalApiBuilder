using Microsoft.CodeAnalysis;

namespace MinimalApiBuilder.Generator.Common;

internal class WellKnownTypes
{
    private static readonly CyclicReferenceCache<Compilation, WellKnownTypes> s_wellKnownTypesCache = new();

    private static readonly string[] s_names =
    {
        "MinimalApiBuilder.MinimalApiBuilderEndpoint",
        "MinimalApiBuilder.RegisterValidatorAttribute",
        "Microsoft.AspNetCore.Builder.RouteHandlerBuilder",
        "Microsoft.AspNetCore.Http.HttpContext",
        "System.IFormatProvider",
        "System.Reflection.ParameterInfo",
        "System.Threading.Tasks.ValueTask`1",
        "FluentValidation.AbstractValidator`1"
    };

    public enum Type
    {
        MinimalApiBuilder_MinimalApiBuilderEndpoint,
        MinimalApiBuilder_RegisterValidatorAttribute,
        Microsoft_AspNetCore_Builder_RouteHandlerBuilder,
        Microsoft_AspNetCore_Http_HttpContext,
        System_IFormatProvider,
        System_Reflection_ParameterInfo,
        System_Threading_Tasks_ValueTask_1,
        FluentValidation_AbstractValidator_1
    }

    private readonly Compilation _compilation;
    private readonly INamedTypeSymbol?[] _types = new INamedTypeSymbol[s_names.Length];

    private WellKnownTypes(Compilation compilation)
    {
        _compilation = compilation;
    }

    public static WellKnownTypes GetOrCreate(Compilation compilation)
    {
        return s_wellKnownTypesCache.GetOrCreateValue(compilation, static c => new WellKnownTypes(c));
    }

    public INamedTypeSymbol this[Type type] => _types[(int)type] ?? GetAndCache((int)type);

    public INamedTypeSymbol this[SpecialType type] => _compilation.GetSpecialType(type);

    private INamedTypeSymbol GetAndCache(int index)
    {
        if (_compilation.GetTypeByMetadataName(s_names[index]) is not { } wellKnownType)
        {
            throw new InvalidOperationException($"Failed to retrieve well-known type: {s_names[index]}");
        }

        Interlocked.CompareExchange(ref _types[index], wellKnownType, null);

        return _types[index]!;
    }
}
