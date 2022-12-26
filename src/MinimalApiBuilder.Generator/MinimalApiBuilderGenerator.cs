using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        IncrementalValuesProvider<ClassDeclarationSyntax> validatorDeclarations =
            context.ForValidatorWithDependenciesDeclarations();
        IncrementalValueProvider<ImmutableArray<ClassDeclarationSyntax>> collectedValidatorDeclarations =
            validatorDeclarations.Collect();

        var source =
            context.CompilationProvider.Combine(collectedEndpointDeclarations.Combine(collectedValidatorDeclarations));

        context.RegisterSourceOutput(source, static (sourceProductionContext, source) =>
            Execute(source.Left, source.Right.Left, source.Right.Right, sourceProductionContext));
    }

    private static void Execute(Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax> endpointDeclarations,
        ImmutableArray<ClassDeclarationSyntax> validatorDeclarations,
        SourceProductionContext context)
    {
        IEnumerable<EndpointToGenerate> endpointsToGenerate =
            EndpointToGenerate.Collect(compilation, endpointDeclarations, context.CancellationToken).ToArray();

        IEnumerable<ValidatorToGenerate> validatorsToGenerate =
            ValidatorToGenerate.Collect(compilation, validatorDeclarations, context.CancellationToken);

        AddDependencyInjectionSource(
            endpointsToGenerate, validatorsToGenerate, context);

        AddEndpointSource(endpointsToGenerate, context);
    }

    private static void AddDependencyInjectionSource(
        IEnumerable<EndpointToGenerate> endpointsToGenerate,
        IEnumerable<ValidatorToGenerate> validatorsToGenerate,
        SourceProductionContext context)
    {
        SourceBuilder sourceBuilder = new();

        SourceBuilder.Block namespaceBlock = sourceBuilder.OpenBlock("namespace MinimalApiBuilder");
        SourceBuilder.Block classBlock = sourceBuilder.OpenBlock("public static class DependencyInjection");
        SourceBuilder.Block methodBlock = sourceBuilder.OpenBlock(
            "public static IServiceCollection AddMinimalApiBuilderEndpoints(this IServiceCollection services)");

        foreach (var endpoint in endpointsToGenerate)
        {
            sourceBuilder.AppendLine($"services.AddScoped<{endpoint.Id}>();");
        }

        foreach (var validator in validatorsToGenerate)
        {
            sourceBuilder.AppendLine($"services.AddTransient<{validator.Id}>();");
        }

        sourceBuilder.AppendLine("return services;");

        methodBlock.Close();
        classBlock.Close();
        namespaceBlock.Close();

        context.AddSource("DependencyInjection.generated.cs", sourceBuilder.ToString());
    }

    private static void AddEndpointSource(
        IEnumerable<EndpointToGenerate> endpointsToGenerate,
        SourceProductionContext context)
    {
        SourceBuilder sourceBuilder = new();

        foreach (var endpoint in endpointsToGenerate)
        {
            SourceBuilder.Block namespaceBlock = sourceBuilder.OpenBlock($"namespace {endpoint.NamespaceName}");
            SourceBuilder.Block classBlock =
                sourceBuilder.OpenBlock(
                    $"public partial class {endpoint.ClassName} : MinimalApiBuilder.IEndpointHandler");

            sourceBuilder.AppendLine($"public static Delegate Handler {{ get; }} = {endpoint.HandlerMethodName};");

            classBlock.Close();
            namespaceBlock.Close();
        }

        context.AddSource("Endpoint.generated.cs", sourceBuilder.ToString());
    }
}
