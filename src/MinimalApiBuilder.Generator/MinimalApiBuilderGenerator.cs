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
        var endpoints = context.ForEndpointDeclarations().Collect();

        IncrementalValuesProvider<ClassDeclarationSyntax> validatorDeclarations = context.ForValidatorDeclarations();
        IncrementalValueProvider<ImmutableArray<ClassDeclarationSyntax>> collectedValidatorDeclarations =
            validatorDeclarations.Collect();

        IncrementalValueProvider<GeneratorOptions> options = context.ForGeneratorOptions();

        var declarations = endpoints.Combine(collectedValidatorDeclarations);

        var source = context.CompilationProvider
            .Combine(declarations)
            .Combine(options);

        context.RegisterSourceOutput(source, static (sourceProductionContext, source) =>
            {
                var compilation = source.Left.Left;
                var (endpoints, validators) = source.Left.Right;
                var options = source.Right;

                Execute(compilation, endpoints,
                    validators, options, sourceProductionContext);
            }
        );
    }

    private static void Execute(Compilation compilation,
        ImmutableArray<EndpointToGenerate> endpoints,
        ImmutableArray<ClassDeclarationSyntax> validatorDeclarations,
        GeneratorOptions options,
        SourceProductionContext context)
    {
        // IEnumerable<EndpointToGenerate> endpoints =
        //     EndpointToGenerate.Collect(compilation, endpointDeclarations, context.CancellationToken);

        IReadOnlyDictionary<string, ValidatorToGenerate> validators =
            ValidatorToGenerate.Collect(compilation, validatorDeclarations, context.CancellationToken);

        AddSource(endpoints, validators, options, context);
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
