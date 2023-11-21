using Microsoft.CodeAnalysis;
using MinimalApiBuilder.Generator.Common;
using MinimalApiBuilder.Generator.Entities;

namespace MinimalApiBuilder.Generator.CodeGeneration.Builders;

internal sealed partial class Endpoints : SourceBuilder
{
    private readonly Dictionary<string, ValidatorToGenerate> _validators;

    public Endpoints(GeneratorOptions options,
        Dictionary<string, ValidatorToGenerate> validators) : base(options)
    {
        _validators = validators;
    }

    public override void AddSource(SourceProductionContext context)
    {
        base.AddSource(context, nameof(Endpoints));
    }

    public void Add(EndpointToGenerate endpoint)
    {
        Options = Options.GetForTarget(endpoint);

        using (endpoint.NamespaceName is null ? Disposable.Empty : OpenBlock($"namespace {endpoint.NamespaceName}"))
        using (OpenBlock(Sources.GeneratedCodeAttribute,
            $"{endpoint.Accessibility.ToStringEnum()} partial class {endpoint.ClassName} : {Fqn.IMinimalApiBuilderEndpoint}"))
        {
            AddConfigure(endpoint);

            if (Options.AssignNameToEndpoint)
            {
                AppendLine(Sources.GeneratedCodeAttribute);
                AppendLine($"public const string Name = \"{endpoint}\";");
            }
        }
    }

    private void AddConfigure(EndpointToGenerate endpoint)
    {
        using (OpenBlock(Sources.GeneratedCodeAttribute,
            $"public static void _auto_generated_Configure({Fqn.RouteHandlerBuilder} builder)"))
        {
            if (Options.AssignNameToEndpoint)
            {
                AppendLine($"{Fqn.WithName}(builder, Name);");
            }

            AddValidationResult result = AddValidation(endpoint);

            if (result is { AnyFilterAdded: false, AnyCustomBinding: true })
            {
                using (OpenAddEndpointFilter())
                {
                    AppendLine(GetEndpoint(endpoint));
                    AddModelBindingFailed();
                    AppendLine($"return {Next};");
                }
            }
        }

        if (endpoint.NeedsConfigure)
        {
            OpenBlock(Sources.GeneratedCodeAttribute,
                $"public static void Configure({Fqn.RouteHandlerBuilder} builder)").Dispose();
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
