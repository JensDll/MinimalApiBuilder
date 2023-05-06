using Microsoft.CodeAnalysis;
using MinimalApiBuilder.Generator.Entities;

namespace MinimalApiBuilder.Generator.CodeGeneration.Builders;

internal class EndpointBuilder : SourceBuilder
{
    private readonly IReadOnlyDictionary<string, ValidatorToGenerate> _validators;

    public EndpointBuilder(GeneratorOptions options, IReadOnlyDictionary<string, ValidatorToGenerate> validators) :
        base(options, "Microsoft.AspNetCore.Builder",
            "MinimalApiBuilder",
            "FluentValidation",
            "FluentValidation.Results")
    {
        _validators = validators;
    }

    public override void AddSource(SourceProductionContext context)
    {
        context.AddSource("Endpoint.generated.cs", ToString());
    }

    public void AddEndpoint(EndpointToGenerate endpoint)
    {
        using (OpenBlock($"namespace {endpoint.NamespaceName}"))
        using (OpenBlock($"public partial class {endpoint.ClassName} : IEndpoint"))
        {
            AddProperties(endpoint);
            AddConfigure(endpoint);

            if (Options.AssignNameToEndpoint)
            {
                AppendLine($"private const string Name = \"{endpoint}\";");
            }
        }
    }

    private void AddProperties(EndpointToGenerate endpoint)
    {
        AppendLine($"public static Delegate _auto_generated_Handler {{ get; }} = {endpoint.Handler.Name};");
    }

    private void AddConfigure(EndpointToGenerate endpoint)
    {
        using (OpenBlock("public static void _auto_generated_Configure(RouteHandlerBuilder builder)"))
        {
            if (Options.AssignNameToEndpoint)
            {
                AppendLine("builder.WithName(Name);");
            }

            AddValidation(endpoint);
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
        using (OpenBlock("builder.AddEndpointFilter(static (invocationContext, next) =>", ");"))
        {
            AppendLine(GetEndpoint(endpointParameter));
            AppendLine($"ValidationResult result = {GetValidationResult(parameter)};");
            AppendLine(
                "return result.IsValid ? next(invocationContext) : ValueTask.FromResult<object?>(IEndpoint.GetErrorResult(endpoint, result));");
        }
    }

    private void AddValidatorsFilter(
        EndpointToGenerateHandlerParameter endpointParameter,
        IEnumerable<EndpointToGenerateHandlerParameter> parameters)
    {
        using (OpenBlock("builder.AddEndpointFilter(static (invocationContext, next) =>", ");"))
        {
            AppendLine(GetEndpoint(endpointParameter));
            AppendLine(
                $"ValidationResult[] results = {{ {string.Join(", ", parameters.Select(GetValidationResult))} }};");
            AppendLine(
                "return results.Any(static result => !result.IsValid) ? ValueTask.FromResult<object?>(IEndpoint.GetErrorResult(endpoint, results)) : next(invocationContext);");
        }
    }

    private void AddAsyncValidatorFilter(
        EndpointToGenerateHandlerParameter endpointParameter,
        EndpointToGenerateHandlerParameter parameter)
    {
        using (OpenBlock("builder.AddEndpointFilter(static async (invocationContext, next) =>", ");"))
        {
            AppendLine(GetEndpoint(endpointParameter));
            AppendLine($"ValidationResult result = await {GetValidationResultAsync(parameter)};");
            AppendLine(
                "return result.IsValid ? await next(invocationContext) : IEndpoint.GetErrorResult(endpoint, result);");
        }
    }

    private void AddAsyncValidatorsFilter(
        EndpointToGenerateHandlerParameter endpointParameter,
        IEnumerable<EndpointToGenerateHandlerParameter> parameters)
    {
        using (OpenBlock("builder.AddEndpointFilter(static async (invocationContext, next) =>", ");"))
        {
            AppendLine(GetEndpoint(endpointParameter));
            AppendLine(
                $"ValidationResult[] results = await Task.WhenAll({string.Join(", ", parameters.Select(GetValidationResultAsync))});");
            AppendLine(
                "return results.Any(static result => !result.IsValid) ? IEndpoint.GetErrorResult(endpoint, results) : await next(invocationContext);");
        }
    }

    private static string GetEndpoint(EndpointToGenerateHandlerParameter endpointParameter) =>
        $"{endpointParameter} endpoint = invocationContext.GetArgument<{endpointParameter}>({endpointParameter.Position});";

    private static string GetValidationResult(EndpointToGenerateHandlerParameter parameter) =>
        $"invocationContext.HttpContext.RequestServices.GetRequiredService<IValidator<{parameter}>>().Validate(invocationContext.GetArgument<{parameter}>({parameter.Position}))";

    private static string GetValidationResultAsync(EndpointToGenerateHandlerParameter parameter) =>
        $"invocationContext.HttpContext.RequestServices.GetRequiredService<IValidator<{parameter}>>().ValidateAsync(invocationContext.GetArgument<{parameter}>({parameter.Position}))";
}
