using Microsoft.CodeAnalysis;
using MinimalApiBuilder.Generator.Common;
using MinimalApiBuilder.Generator.Entities;

namespace MinimalApiBuilder.Generator.CodeGeneration.Builders;

internal class DependencyInjectionBuilder : SourceBuilder
{
    private readonly IDisposable _namespace;
    private readonly IDisposable _class;
    private readonly IDisposable _method;

    public DependencyInjectionBuilder(GeneratorOptions options) : base(options)
    {
        _namespace = OpenBlock("namespace MinimalApiBuilder");
        _class = OpenBlock(
            "/// <summary>",
            "/// Minimal API builder dependency injection extension methods.",
            "/// </summary>",
            s_generatedCodeAttribute,
            "public static class MinimalApiBuilderDependencyInjectionExtensions");
        _method = OpenBlock(
            "/// <summary>",
            $"/// Adds the necessary types to the <see cref=\"{Fqn.IServiceCollection}\"/>.",
            "/// </summary>",
            $"/// <param name=\"services\">The <see cref=\"{Fqn.IServiceCollection}\"/>.</param>",
            s_generatedCodeAttribute,
            $"public static {Fqn.IServiceCollection} AddMinimalApiBuilderEndpoints(this {Fqn.IServiceCollection} services)");
    }

    public override void AddSource(SourceProductionContext context)
    {
        AppendLine("return services;");
        _method.Dispose();
        _class.Dispose();
        _namespace.Dispose();
        context.AddSource("MinimalApiBuilderDependencyInjectionExtensions.g.cs", ToString());
    }

    public void AddService(EndpointToGenerate endpoint)
    {
        AppendAddService("Scoped", endpoint.ToString());
    }

    public void AddService(KeyValuePair<string, ValidatorToGenerate> entry)
    {
        string validatedType = entry.Key;
        ValidatorToGenerate validator = entry.Value;
        AppendAddService(validator.ServiceLifetime, $"{Fqn.IValidator}<{validatedType}>, {validator}");
    }

    private void AppendAddService(string lifetime, string generic) =>
        AppendLine(
            $"global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.Add{lifetime}<{generic}>(services);");
}
