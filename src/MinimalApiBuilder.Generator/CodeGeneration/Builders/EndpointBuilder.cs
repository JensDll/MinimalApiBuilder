using Microsoft.CodeAnalysis;
using MinimalApiBuilder.Generator.Entities;

namespace MinimalApiBuilder.Generator.CodeGeneration.Builders;

internal class EndpointBuilder : SourceBuilder
{
    private readonly IReadOnlyDictionary<string, ValidatorToGenerate> _validators;

    public EndpointBuilder(GeneratorOptions options, IReadOnlyDictionary<string, ValidatorToGenerate> validators)
        : base(options)
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
        using (OpenBlock($"public partial class {endpoint.ClassName} : {FullyQualifiedNames.IEndpoint}"))
        {
            AddProperties(endpoint);
            AddConfigure(endpoint);

            if (Options.AssignNameToEndpoint)
            {
                MarkAsGenerated();
                AppendLine($"private const string Name = \"{endpoint}\";");
            }
        }
    }

    private void AddProperties(EndpointToGenerate endpoint)
    {
        MarkAsGenerated();
        AppendLine(
            $"public static {FullyQualifiedNames.Delegate} _auto_generated_Handler {{ get; }} = {endpoint.Handler.Name};");
    }

    private void AddConfigure(EndpointToGenerate endpoint)
    {
        MarkAsGenerated();
        using (OpenBlock(
                   $"public static void _auto_generated_Configure({FullyQualifiedNames.RouteHandlerBuilder} builder)"))
        {
            if (Options.AssignNameToEndpoint)
            {
                AppendLine("builder.WithName(Name);");
            }

            AddValidation(endpoint);
        }

        if (endpoint.NeedsConfigure)
        {
            MarkAsGenerated();
            OpenBlock($"public static void Configure({FullyQualifiedNames.RouteHandlerBuilder} builder)").Dispose();
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
            AppendLine($"{FullyQualifiedNames.ValidationResult} result = {GetValidationResult(parameter)};");
            AppendLine(
                $"return result.IsValid ? next(invocationContext) : {FullyQualifiedNames.ValueTask}.FromResult<object?>({FullyQualifiedNames.IEndpoint}.GetErrorResult(endpoint, result));");
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
                $"{FullyQualifiedNames.ValidationResult}[] results = {{ {string.Join(", ", parameters.Select(GetValidationResult))} }};");
            AppendLine(
                $"return {FullyQualifiedNames.LinqAny("results", "static result => !result.IsValid")} ? {FullyQualifiedNames.ValueTask}.FromResult<object?>({FullyQualifiedNames.IEndpoint}.GetErrorResult(endpoint, results)) : next(invocationContext);");
        }
    }

    private void AddAsyncValidatorFilter(
        EndpointToGenerateHandlerParameter endpointParameter,
        EndpointToGenerateHandlerParameter parameter)
    {
        using (OpenAddEndpointFilterAsync())
        {
            AppendLine(GetEndpoint(endpointParameter));
            AppendLine($"{FullyQualifiedNames.ValidationResult} result = await {GetValidationResultAsync(parameter)};");
            AppendLine(
                $"return result.IsValid ? await next(invocationContext) : {FullyQualifiedNames.IEndpoint}.GetErrorResult(endpoint, result);");
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
                $"{FullyQualifiedNames.ValidationResult}[] results = await {FullyQualifiedNames.Task}.WhenAll({string.Join(", ", parameters.Select(GetValidationResultAsync))});");
            AppendLine(
                $"return {FullyQualifiedNames.LinqAny("results", "static result => !result.IsValid")} ? {FullyQualifiedNames.IEndpoint}.GetErrorResult(endpoint, results) : await next(invocationContext);");
        }
    }

    private static string GetEndpoint(EndpointToGenerateHandlerParameter endpointParameter) =>
        $"{endpointParameter} endpoint = invocationContext.GetArgument<{endpointParameter}>({endpointParameter.Position.ToString()});";

    private static string GetValidationResult(EndpointToGenerateHandlerParameter parameter) =>
        $"{GetRequiredService($"{FullyQualifiedNames.IValidator}<{parameter}>")}.Validate(invocationContext.GetArgument<{parameter}>({parameter.Position.ToString()}))";

    private static string GetValidationResultAsync(EndpointToGenerateHandlerParameter parameter) =>
        $"{GetRequiredService($"{FullyQualifiedNames.IValidator}<{parameter}>")}.ValidateAsync(invocationContext.GetArgument<{parameter}>({parameter.Position.ToString()}))";

    private static string GetRequiredService(string type) =>
        $"global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<{type}>(invocationContext.HttpContext.RequestServices)";

    private IDisposable OpenAddEndpointFilter() => OpenBlock(
        "global::Microsoft.AspNetCore.Http.EndpointFilterExtensions.AddEndpointFilter(builder, static (invocationContext, next) =>",
        ");");

    private IDisposable OpenAddEndpointFilterAsync() => OpenBlock(
        "global::Microsoft.AspNetCore.Http.EndpointFilterExtensions.AddEndpointFilter(builder, static async (invocationContext, next) =>",
        ");");
}
