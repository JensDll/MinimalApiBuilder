using Microsoft.CodeAnalysis;
using MinimalApiBuilder.Generator.Common;
using MinimalApiBuilder.Generator.Entities;

namespace MinimalApiBuilder.Generator.CodeGeneration.Builders;

internal sealed class MapEndpointsExtensions : SourceBuilder
{
    private static readonly string[] s_mapActions = { "MapGet", "MapPost", "MapPut", "MapDelete", "MapPatch" };

    private readonly IDisposable _namespace;
    private readonly IDisposable _class;

    public MapEndpointsExtensions(GeneratorOptions options) : base(options)
    {
        _namespace = OpenBlock("namespace MinimalApiBuilder");
        _class = OpenBlock(
            s_generatedCodeAttribute,
            $"internal static class {nameof(MapEndpointsExtensions)}");
    }

    public override void AddSource(SourceProductionContext context)
    {
        _class.Dispose();
        _namespace.Dispose();
        context.AddSource($"{nameof(MapEndpointsExtensions)}.g.cs", ToString());
        base.AddSource(context);
    }

    public void Add(EndpointToGenerate endpoint)
    {
        using (OpenBlock(
            s_generatedCodeAttribute,
            $"public static {Fqn.RouteHandlerBuilder} Map<TEndpoint>(this {Fqn.IEndpointRouteBuilder} endpoints, [{Fqn.StringSyntax}(\"Route\")] string pattern, {endpoint}? _ = default) where TEndpoint : {endpoint}"))
        {
            AppendLine($"{Fqn.RouteHandlerBuilder} builder = global::Microsoft.AspNetCore.Builder.EndpointRouteBuilderExtensions.Map(endpoints, pattern, {endpoint}.{endpoint.Handler.Name});");
            AppendLine($"{endpoint}._auto_generated_Configure(builder);");
            AppendLine($"{endpoint}.Configure(builder);");
            AppendLine("return builder;");
        }

        using (OpenBlock(
            s_generatedCodeAttribute,
            $"public static {Fqn.RouteHandlerBuilder} MapMethods<TEndpoint>(this {Fqn.IEndpointRouteBuilder} endpoints, [{Fqn.StringSyntax}(\"Route\")] string pattern, {Fqn.IEnumerable}<string> httpMethods, {endpoint}? _ = default) where TEndpoint : {endpoint}"))
        {
            AppendLine($"{Fqn.RouteHandlerBuilder} builder = global::Microsoft.AspNetCore.Builder.EndpointRouteBuilderExtensions.MapMethods(endpoints, pattern, httpMethods, {endpoint}.{endpoint.Handler.Name});");
            AppendLine($"{endpoint}._auto_generated_Configure(builder);");
            AppendLine($"{endpoint}.Configure(builder);");
            AppendLine("return builder;");
        }

        foreach (string mapAction in s_mapActions)
        {
            using (OpenBlock(
                s_generatedCodeAttribute,
                $"public static {Fqn.RouteHandlerBuilder} {mapAction}<TEndpoint>(this {Fqn.IEndpointRouteBuilder} endpoints, [{Fqn.StringSyntax}(\"Route\")] string pattern, {endpoint}? _ = default) where TEndpoint : {endpoint}"))
            {
                AppendLine($"{Fqn.RouteHandlerBuilder} builder = global::Microsoft.AspNetCore.Builder.EndpointRouteBuilderExtensions.{mapAction}(endpoints, pattern, {endpoint}.{endpoint.Handler.Name});");
                AppendLine($"{endpoint}._auto_generated_Configure(builder);");
                AppendLine($"{endpoint}.Configure(builder);");
                AppendLine("return builder;");
            }
        }
    }
}
