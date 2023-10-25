using Microsoft.CodeAnalysis;
using MinimalApiBuilder.Generator.Common;
using MinimalApiBuilder.Generator.Entities;

namespace MinimalApiBuilder.Generator.CodeGeneration.Builders;

internal class EndpointBuilder : SourceBuilder
{
    private readonly IReadOnlyDictionary<string, ValidatorToGenerate> _validators;

    public EndpointBuilder(GeneratorOptions options,
        IReadOnlyDictionary<string, ValidatorToGenerate> validators) : base(options)
    {
        _validators = validators;
    }

    public override void AddSource(SourceProductionContext context)
    {
        context.AddSource("Endpoint.g.cs", ToString());
    }

    public void AddEndpoint(EndpointToGenerate endpoint)
    {
        using (endpoint.NamespaceName is null ? Disposable.Empty : OpenBlock($"namespace {endpoint.NamespaceName}"))
        using (OpenBlock($"public partial class {endpoint.ClassName} : {Fqn.IEndpoint}"))
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
        AppendLine(
            $"public static {Fqn.Delegate} _auto_generated_Handler {{ get; }} = {endpoint.Handler.Name};");
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

            AddValidation(endpoint);
        }

        if (endpoint.NeedsConfigure)
        {
            MarkAsGenerated();
            OpenBlock($"public static void Configure({Fqn.RouteHandlerBuilder} builder)").Dispose();
        }
    }

    private void AddValidation(EndpointToGenerate endpoint)
    {
        List<EndpointToGenerateHandlerParameter> parametersToValidateSync = new();
        List<EndpointToGenerateHandlerParameter> parametersToValidateAsync = new();

        foreach (var parameter in endpoint.Handler.Parameters)
        {
            if (!_validators.TryGetValue(parameter.ToString(), out var validator))
            {
                continue;
            }

            if (validator.IsAsync)
            {
                parametersToValidateAsync.Add(parameter);
            }
            else
            {
                parametersToValidateSync.Add(parameter);
            }
        }

        switch (parametersToValidateSync.Count)
        {
            case 1:
                AddValidatorFilter(endpoint.Handler.EndpointParameter, parametersToValidateSync[0]);
                break;
            case > 1:
                AddValidatorsFilter(endpoint.Handler.EndpointParameter, parametersToValidateSync);
                break;
        }

        switch (parametersToValidateAsync.Count)
        {
            case 1:
                AddAsyncValidatorFilter(endpoint.Handler.EndpointParameter, parametersToValidateAsync[0]);
                break;
            case > 1:
                AddAsyncValidatorsFilter(endpoint.Handler.EndpointParameter, parametersToValidateAsync);
                break;
        }
    }

    private void AddValidatorFilter(
        EndpointToGenerateHandlerParameter endpointParameter,
        EndpointToGenerateHandlerParameter parameter)
    {
        using (OpenAddEndpointFilter())
        {
            AppendLine(GetEndpoint(endpointParameter));
            AppendLine($"{Fqn.ValidationResult} result = {GetValidationResult(parameter)};");
            AppendLine(
                $"return result.IsValid ? next(invocationContext) : {Fqn.ValueTask}.FromResult<object?>({Fqn.IEndpoint}.GetErrorResult(endpoint, result));");
        }
    }

    private void AddValidatorsFilter(
        EndpointToGenerateHandlerParameter endpointParameter,
        IEnumerable<EndpointToGenerateHandlerParameter> parameters)
    {
        using (OpenAddEndpointFilter())
        {
            AppendLine(GetEndpoint(endpointParameter));
            AppendLine(
                $"{Fqn.ValidationResult}[] results = {{ {string.Join(", ", parameters.Select(GetValidationResult))} }};");
            AppendLine(
                $"return {Fqn.Linq}.Any(results, static result => !result.IsValid) ? {Fqn.ValueTask}.FromResult<object?>({Fqn.IEndpoint}.GetErrorResult(endpoint, results)) : next(invocationContext);");
        }
    }

    private void AddAsyncValidatorFilter(
        EndpointToGenerateHandlerParameter endpointParameter,
        EndpointToGenerateHandlerParameter parameter)
    {
        using (OpenAddEndpointFilterAsync())
        {
            AppendLine(GetEndpoint(endpointParameter));
            AppendLine($"{Fqn.ValidationResult} result = await {GetValidationResultAsync(parameter)};");
            AppendLine(
                $"return result.IsValid ? await next(invocationContext) : {Fqn.IEndpoint}.GetErrorResult(endpoint, result);");
        }
    }

    private void AddAsyncValidatorsFilter(
        EndpointToGenerateHandlerParameter endpointParameter,
        IEnumerable<EndpointToGenerateHandlerParameter> parameters)
    {
        using (OpenAddEndpointFilterAsync())
        {
            AppendLine(GetEndpoint(endpointParameter));
            AppendLine(
                $"{Fqn.ValidationResult}[] results = await {Fqn.Task}.WhenAll({string.Join(", ", parameters.Select(GetValidationResultAsync))});");
            AppendLine(
                $"return {Fqn.Linq}.Any(results, static result => !result.IsValid) ? {Fqn.IEndpoint}.GetErrorResult(endpoint, results) : await next(invocationContext);");
        }
    }

    private static string GetEndpoint(EndpointToGenerateHandlerParameter endpointParameter) =>
        $"{endpointParameter} endpoint = invocationContext.GetArgument<{endpointParameter}>({endpointParameter.Position});";

    private static string GetValidationResult(EndpointToGenerateHandlerParameter parameter) =>
        $"{GetRequiredService($"{Fqn.IValidator}<{parameter}>")}.Validate(invocationContext.GetArgument<{parameter}>({parameter.Position}))";

    private static string GetValidationResultAsync(EndpointToGenerateHandlerParameter parameter) =>
        $"{GetRequiredService($"{Fqn.IValidator}<{parameter}>")}.ValidateAsync(invocationContext.GetArgument<{parameter}>({parameter.Position}))";

    private static string GetRequiredService(string type) =>
        $"{Fqn.GetRequiredService}<{type}>(invocationContext.HttpContext.RequestServices)";

    private IDisposable OpenAddEndpointFilter() => OpenBlock(
        $"{Fqn.AddEndpointFilter}(builder, static (invocationContext, next) =>",
        ");");

    private IDisposable OpenAddEndpointFilterAsync() => OpenBlock(
        $"{Fqn.AddEndpointFilter}(builder, static async (invocationContext, next) =>",
        ");");
}
