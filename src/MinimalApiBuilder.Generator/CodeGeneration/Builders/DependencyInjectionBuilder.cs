using Microsoft.CodeAnalysis;
using MinimalApiBuilder.Generator.Entities;

namespace MinimalApiBuilder.Generator.CodeGeneration.Builders;

internal class DependencyInjectionBuilder : SourceBuilder
{
    private readonly IDisposable _namespaceDisposable;
    private readonly IDisposable _classDisposable;
    private readonly IDisposable _methodDisposable;

    public DependencyInjectionBuilder() : base("FluentValidation",
        "Microsoft.Extensions.DependencyInjection")
    {
        _namespaceDisposable = OpenBlock("namespace MinimalApiBuilder");
        _classDisposable = OpenBlock("public static class DependencyInjection");
        _methodDisposable =
            OpenBlock(
                "public static IServiceCollection AddMinimalApiBuilderEndpoints(this IServiceCollection services)");
    }

    public override void AddSource(SourceProductionContext context)
    {
        AppendLine("return services;");
        _methodDisposable.Dispose();
        _classDisposable.Dispose();
        _namespaceDisposable.Dispose();
        context.AddSource("DependencyInjection.generated.cs", ToString());
    }

    public void AddService(EndpointToGenerate endpoint)
    {
        AppendLine($"services.AddScoped<{endpoint}>();");
    }

    public void AddService(KeyValuePair<string, ValidatorToGenerate> entry)
    {
        string validatedType = entry.Key;
        ValidatorToGenerate validator = entry.Value;

        AppendLine($"services.Add{validator.ServiceLifetime}<IValidator<{validatedType}>, {validator}>();");
    }
}
