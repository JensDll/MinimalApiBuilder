﻿using Microsoft.CodeAnalysis;
using MinimalApiBuilder.Generator.Common;
using MinimalApiBuilder.Generator.Entities;

namespace MinimalApiBuilder.Generator.CodeGeneration.Builders;

internal sealed class DependencyInjectionExtensions : SourceBuilder
{
    private readonly IDisposable _namespace;
    private readonly IDisposable _class;
    private readonly IDisposable _method;

    public DependencyInjectionExtensions(GeneratorOptions options) : base(options)
    {
        _namespace = OpenBlock("namespace MinimalApiBuilder.Generator");
        _class = OpenBlock(
            "/// <summary>",
            "/// Minimal API builder dependency injection extension methods.",
            "/// </summary>",
            Sources.GeneratedCodeAttribute,
            $"internal static class {nameof(DependencyInjectionExtensions)}");
        _method = OpenBlock(
            "/// <summary>",
            $"/// Adds the necessary types to the <see cref=\"{Fqn.IServiceCollection}\"/>.",
            "/// </summary>",
            $"/// <param name=\"services\">The <see cref=\"{Fqn.IServiceCollection}\"/>.</param>",
            Sources.GeneratedCodeAttribute,
            $"public static {Fqn.IServiceCollection} AddMinimalApiBuilderEndpoints(this {Fqn.IServiceCollection} services)");
    }

    public override void AddSource(SourceProductionContext context)
    {
        AppendLine("return services;");
        _method.Dispose();
        _class.Dispose();
        _namespace.Dispose();
        base.AddSource(context, nameof(DependencyInjectionExtensions));
    }

    public void Add(EndpointToGenerate endpoint)
    {
        AppendAddService("Scoped", endpoint.ToString());
    }

    public void Add(KeyValuePair<string, ValidatorToGenerate> entry)
    {
        string validatedType = entry.Key;
        ValidatorToGenerate validator = entry.Value;
        AppendAddService(validator.ServiceLifetime.ToStringServiceLifetime(),
            $"{Fqn.IValidator}<{validatedType}>, {validator}");
    }

    private void AppendAddService(string lifetime, string generic)
    {
        AppendLine(
            $"global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.Add{lifetime}<{generic}>(services);");
    }
}
