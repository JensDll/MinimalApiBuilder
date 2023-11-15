using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using MinimalApiBuilder.Generator.CodeGeneration.Builders;
using MinimalApiBuilder.Generator.Entities;
using MinimalApiBuilder.Generator.Providers;

namespace MinimalApiBuilder.Generator;

[Generator(LanguageNames.CSharp)]
internal sealed class MinimalApiBuilderGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<ImmutableArray<EndpointToGenerate>> endpoints = context.ForEndpoints().Collect();
        IncrementalValueProvider<ImmutableArray<ValidatorToGenerate>> validators = context.ForValidators().Collect();
        IncrementalValueProvider<GeneratorOptions> options = context.ForGeneratorOptions();

        var sourceProvider = endpoints.Combine(validators).Combine(options);

        context.RegisterSourceOutput(sourceProvider, static (sourceProductionContext, source) =>
            Execute(source.Left.Left, source.Left.Right, source.Right, sourceProductionContext));
    }

    private static void Execute(ImmutableArray<EndpointToGenerate> endpoints,
        ImmutableArray<ValidatorToGenerate> validators,
        GeneratorOptions options,
        SourceProductionContext context)
    {
        AddSource(endpoints, validators.ToDictionary(static validator => validator.ValidatedType), options, context);
    }

    private static void AddSource(
        ImmutableArray<EndpointToGenerate> endpoints,
        Dictionary<string, ValidatorToGenerate> validators,
        GeneratorOptions options,
        SourceProductionContext context)
    {
        Endpoints endpointsBuilder = new(options, validators);
        MapEndpointsExtensions mapEndpointsBuilder = new(options);
        DependencyInjectionExtensions dependencyInjectionExtensionsBuilder = new(options);

        foreach (EndpointToGenerate endpoint in endpoints)
        {
            endpointsBuilder.Add(endpoint);
            mapEndpointsBuilder.Add(endpoint);
            dependencyInjectionExtensionsBuilder.Add(endpoint);
        }

        foreach (KeyValuePair<string, ValidatorToGenerate> entry in validators)
        {
            dependencyInjectionExtensionsBuilder.Add(entry);
        }

        endpointsBuilder.AddSource(context);
        mapEndpointsBuilder.AddSource(context);
        dependencyInjectionExtensionsBuilder.AddSource(context);
    }
}
