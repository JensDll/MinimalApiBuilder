using Microsoft.CodeAnalysis;
using MinimalApiBuilder.Generator.Common;
using MinimalApiBuilder.Generator.Entities;

namespace MinimalApiBuilder.Generator.CodeGeneration.Builders;

internal partial class EndpointBuilder : SourceBuilder
{
    private readonly IReadOnlyDictionary<string, ValidatorToGenerate> _validators;

    public EndpointBuilder(GeneratorOptions options,
        IReadOnlyDictionary<string, ValidatorToGenerate> validators) : base(options)
    {
        _validators = validators;
    }

    public override void AddSource(SourceProductionContext context)
    {
        context.AddSource("MinimalApiBuilderEndpoints.g.cs", ToString());
    }

    public void AddEndpoint(EndpointToGenerate endpoint)
    {
        Options = Options.GetForTarget(endpoint);

        using (endpoint.NamespaceName is null ? Disposable.Empty : OpenBlock($"namespace {endpoint.NamespaceName}"))
        using (OpenBlock(
                   s_generatedCodeAttribute,
                   $"{endpoint.Accessibility.ToAccessibilityString()} partial class {endpoint.ClassName} : {Fqn.IMinimalApiBuilderEndpoint}"))
        {
            AddProperties(endpoint);
            AddConfigure(endpoint);

            if (Options.AssignNameToEndpoint)
            {
                MarkAsGenerated();
                AppendLine($"public const string Name = \"{endpoint}\";");
            }
        }
    }

    private void AddProperties(EndpointToGenerate endpoint)
    {
        MarkAsGenerated();
        AppendLine($"public static {Fqn.Delegate} _auto_generated_Handler {{ get; }} = {endpoint.Handler.Name};");
    }

    private void AddConfigure(EndpointToGenerate endpoint)
    {
        MarkAsGenerated();
        using (OpenBlock($"public static void _auto_generated_Configure({Fqn.RouteHandlerBuilder} builder)"))
        {
            if (Options.AssignNameToEndpoint)
            {
                AppendLine($"{Fqn.WithName}(builder, Name);");
            }

            bool anyAdded = AddValidation(endpoint);

            if (!anyAdded && endpoint.Handler.Parameters.Any(static parameter => parameter.HasCustomBinding))
            {
                using IDisposable filterBlock = OpenAddEndpointFilter();
                AppendLine(GetEndpoint(endpoint.Handler.EndpointParameter));
                AddModelBindingFailed();
                AppendLine($"return {Next};");
            }
        }

        if (endpoint.NeedsConfigure)
        {
            MarkAsGenerated();
            OpenBlock($"public static void Configure({Fqn.RouteHandlerBuilder} builder)").Dispose();
        }
    }

    private void AddModelBindingFailed()
    {
        using IDisposable ifBlock = OpenBlock("if (endpoint.HasValidationError)");
        AppendLine($"return {Fqn.ValueTask}.FromResult<object?>({ModelBindingFailed()});");
    }

    private void AddAsyncModelBindingFailed()
    {
        using IDisposable ifBlock = OpenBlock("if (endpoint.HasValidationError)");
        AppendLine($"return {ModelBindingFailed()};");
    }
}
