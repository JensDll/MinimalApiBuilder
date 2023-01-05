using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MinimalApiBuilder.Generator.CodeGeneration.Builders;
using MinimalApiBuilder.Generator.Entities;
using MinimalApiBuilder.Generator.Providers;

namespace MinimalApiBuilder.Generator;

[Generator(LanguageNames.CSharp)]
internal class MinimalApiBuilderGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<ClassDeclarationSyntax> endpointDeclarations = context.ForEndpointDeclarations();
        IncrementalValueProvider<ImmutableArray<ClassDeclarationSyntax>> collectedEndpointDeclarations =
            endpointDeclarations.Collect();

        IncrementalValuesProvider<ClassDeclarationSyntax> validatorDeclarations = context.ForValidatorDeclarations();
        IncrementalValueProvider<ImmutableArray<ClassDeclarationSyntax>> collectedValidatorDeclarations =
            validatorDeclarations.Collect();

        var source =
            context.CompilationProvider.Combine(collectedEndpointDeclarations.Combine(collectedValidatorDeclarations));

        context.RegisterSourceOutput(source, static (sourceProductionContext, source) =>
            Execute(source.Left, source.Right.Left,
                source.Right.Right, sourceProductionContext));
    }

    private static void Execute(Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax> endpointDeclarations,
        ImmutableArray<ClassDeclarationSyntax> validatorDeclarations,
        SourceProductionContext context)
    {
        IEnumerable<EndpointToGenerate> endpoints =
            EndpointToGenerate.Collect(compilation, endpointDeclarations, context.CancellationToken);

        IReadOnlyDictionary<string, ValidatorToGenerate> validators =
            ValidatorToGenerate.Collect(compilation, validatorDeclarations, context.CancellationToken);

        AddSource(endpoints, validators, context);
    }

    private static void AddSource(
        IEnumerable<EndpointToGenerate> endpoints,
        IReadOnlyDictionary<string, ValidatorToGenerate> validators,
        SourceProductionContext context)
    {
        EndpointBuilder endpointBuilder = new(validators);
        DependencyInjectionBuilder dependencyInjectionBuilder = new();

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
