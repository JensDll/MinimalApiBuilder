using Microsoft.CodeAnalysis;
using MinimalApiBuilder.Generator.Entities;

namespace MinimalApiBuilder.Generator.CodeGeneration.Builders;

internal class DependencyInjectionBuilder : SourceBuilder
{
    private readonly IDisposable _namespaceDisposable;
    private readonly IDisposable _classDisposable;
    private readonly IDisposable _methodDisposable;

    public DependencyInjectionBuilder(GeneratorOptions options) : base(options)
    {
        _namespaceDisposable = OpenBlock("namespace MinimalApiBuilder");
        _classDisposable = OpenBlock("public static class DependencyInjection");
        MarkAsGenerated();
        _methodDisposable =
            OpenBlock(
                $"public static {Fqn.IServiceCollection} AddMinimalApiBuilderEndpoints(this {Fqn.IServiceCollection} services)");
    }

    public override void AddSource(SourceProductionContext context)
    {
        AppendLine("return services;");
        _methodDisposable.Dispose();
        _classDisposable.Dispose();
        _namespaceDisposable.Dispose();
        context.AddSource("DependencyInjection.g.cs", ToString());
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
