using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using MinimalApiBuilder.Generator.CodeGeneration.Builders;
using MinimalApiBuilder.Generator.Common;
using MinimalApiBuilder.Generator.Entities;
using MinimalApiBuilder.Generator.Providers;

namespace MinimalApiBuilder.Generator;

[Generator(LanguageNames.CSharp)]
internal sealed class MinimalApiBuilderGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var endpoints = context.ForEndpoints().Collect();
        var validators = context.ForValidators().Collect();
        var options = context.ForGeneratorOptions();
        var configures = context.ForConfigure().Collect();

        var endpointsAndValidatorsAndOptions = endpoints.Combine(validators).Combine(options);

        context.RegisterSourceOutput(endpointsAndValidatorsAndOptions, static (sourceProductionContext, source) =>
            Execute(source.Left.Left, source.Left.Right, source.Right, sourceProductionContext));

        context.RegisterSourceOutput(configures, static (sourceProductionContext, source) =>
            Execute(source, sourceProductionContext));

        context.RegisterPostInitializationOutput(static postInitContext =>
        {
            postInitContext.AddSource($"{nameof(Sources.ConfigureEndpoints)}.g.cs", Sources.ConfigureEndpoints);
        });
    }

    private static void Execute(ImmutableArray<EndpointToGenerate> endpoints,
        ImmutableArray<ValidatorToGenerate> validators,
        GeneratorOptions options,
        SourceProductionContext context)
    {
        AddSource(endpoints, validators.ToDictionary(static validator => validator.ValidatedType), options, context);
    }

    private static void Execute(
        ImmutableArray<ConfigureToGenerate> configures,
        SourceProductionContext context)
    {
        if (configures.Length == 0)
        {
            return;
        }

        AddSource(configures.GroupBy(static value => value.Arity).ToImmutableArray(), context);
    }

    private static void AddSource(
        ImmutableArray<EndpointToGenerate> endpoints,
        Dictionary<string, ValidatorToGenerate> validators,
        GeneratorOptions options,
        SourceProductionContext context)
    {
        Endpoints endpointsBuilder = new(options, validators);
        DependencyInjectionExtensions dependencyInjectionExtensionsBuilder = new(options);

        foreach (EndpointToGenerate endpoint in endpoints)
        {
            endpointsBuilder.Add(endpoint);
            dependencyInjectionExtensionsBuilder.Add(endpoint);
        }

        foreach (KeyValuePair<string, ValidatorToGenerate> entry in validators)
        {
            dependencyInjectionExtensionsBuilder.Add(entry);
        }

        endpointsBuilder.AddSource(context);
        dependencyInjectionExtensionsBuilder.AddSource(context);
    }

    private static void AddSource(
        IEnumerable<IGrouping<int, ConfigureToGenerate>> configures,
        SourceProductionContext context)
    {
        ConfigureEndpoints configureEndpointsBuilder = new();

        foreach (var group in configures)
        {
            configureEndpointsBuilder.Add(group.ToImmutableArray());
        }

        configureEndpointsBuilder.AddSource(context);
    }
}
