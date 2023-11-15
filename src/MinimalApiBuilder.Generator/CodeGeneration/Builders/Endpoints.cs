using Microsoft.CodeAnalysis;
using MinimalApiBuilder.Generator.Common;
using MinimalApiBuilder.Generator.Entities;

namespace MinimalApiBuilder.Generator.CodeGeneration.Builders;

internal partial class Endpoints : SourceBuilder
{
    private readonly Dictionary<string, ValidatorToGenerate> _validators;

    public Endpoints(GeneratorOptions options,
        Dictionary<string, ValidatorToGenerate> validators) : base(options)
    {
        _validators = validators;
    }

    public override void AddSource(SourceProductionContext context)
    {
        context.AddSource($"{nameof(Endpoints)}.g.cs", ToString());
        base.AddSource(context);
    }

    public void Add(EndpointToGenerate endpoint)
    {
        Options = Options.GetForTarget(endpoint);

        using (endpoint.NamespaceName is null ? Disposable.Empty : OpenBlock($"namespace {endpoint.NamespaceName}"))
        using (OpenBlock(s_generatedCodeAttribute,
            $"{endpoint.Accessibility.ToStringEnum()} partial class {endpoint.ClassName}"))
        {
            AddConfigure(endpoint);

            if (Options.AssignNameToEndpoint)
            {
                AppendLine(s_generatedCodeAttribute);
                AppendLine($"public const string Name = \"{endpoint}\";");
            }
        }
    }

    private void AddConfigure(EndpointToGenerate endpoint)
    {
        using (OpenBlock(s_generatedCodeAttribute,
            $"public static void _auto_generated_Configure({Fqn.RouteHandlerBuilder} builder)"))
        {
            if (Options.AssignNameToEndpoint)
            {
                AppendLine($"{Fqn.WithName}(builder, Name);");
            }

            AddValidationResult result = AddValidation(endpoint);

            if (result is { AnyFilterAdded: false, AnyCustomBinding: true })
            {
                using IDisposable filterBlock = OpenAddEndpointFilter();
                AppendLine(GetEndpoint(endpoint.Handler.EndpointParameter));
                AddModelBindingFailed();
                AppendLine($"return {Next};");
            }
        }

        if (endpoint.NeedsConfigure)
        {
            OpenBlock(s_generatedCodeAttribute, $"public static void Configure({Fqn.RouteHandlerBuilder} builder)").Dispose();
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
