using Microsoft.CodeAnalysis;
using MinimalApiBuilder.Generator.Entities;

namespace MinimalApiBuilder.Generator.CodeGeneration;

internal class MainBuilder : SourceBuilder
{
    private readonly IEnumerable<EndpointToGenerate> _endpoints;
    private readonly IReadOnlyDictionary<string, ValidatorToGenerate> _validators;

    public MainBuilder(
        IEnumerable<EndpointToGenerate> endpoints,
        IReadOnlyDictionary<string, ValidatorToGenerate> validators)
        : base("FluentValidation")
    {
        _endpoints = endpoints;
        _validators = validators;
    }

    public override void AddSource(SourceProductionContext context)
    {
        EndpointBuilder endpointBuilder = new(_validators);

        using (OpenBlock("namespace MinimalApiBuilder"))
        using (OpenBlock("public static class DependencyInjection"))
        using (OpenBlock(
                   "public static IServiceCollection AddMinimalApiBuilderEndpoints(this IServiceCollection services)"))
        {
            foreach (EndpointToGenerate endpoint in _endpoints)
            {
                AppendLine($"services.AddScoped<{endpoint}>();");
                endpointBuilder.AddEndpoint(endpoint);
            }

            foreach (KeyValuePair<string, ValidatorToGenerate> entry in _validators)
            {
                AppendLine($"services.AddSingleton<IValidator<{entry.Key}>, {entry.Value}>();");
            }

            AppendLine("return services;");
        }

        endpointBuilder.AddSource(context);
        context.AddSource("DependencyInjection.generated.cs", ToString());
    }
}
