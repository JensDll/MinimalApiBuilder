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
        var endpoints = context.ForEndpoints().Collect();
        var validators = context.ForValidators().Collect();
        var options = context.ForGeneratorOptions();

        var source = endpoints.Combine(validators).Combine(options);

        context.RegisterSourceOutput(source, static (sourceProductionContext, source) =>
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
        IReadOnlyDictionary<string, ValidatorToGenerate> validators,
        GeneratorOptions options,
        SourceProductionContext context)
    {
        EndpointBuilder endpointBuilder = new(options, validators);
        DependencyInjectionBuilder dependencyInjectionBuilder = new(options);

        foreach (EndpointToGenerate endpoint in endpoints)
        {
            dependencyInjectionBuilder.AddService(endpoint);
            endpointBuilder.AddEndpoint(endpoint);
        }

        foreach (KeyValuePair<string, ValidatorToGenerate> entry in validators)
        {
            dependencyInjectionBuilder.AddService(entry);
        }

        dependencyInjectionBuilder.AddSource(context);
        endpointBuilder.AddSource(context);
    }
}
