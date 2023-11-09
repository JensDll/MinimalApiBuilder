using Microsoft.CodeAnalysis;
using MinimalApiBuilder.Generator.Common;
using MinimalApiBuilder.Generator.Entities;

namespace MinimalApiBuilder.Generator.CodeGeneration.Builders;

internal partial class EndpointBuilder
{
    private bool AddValidation(EndpointToGenerate endpoint)
    {
        List<EndpointToGenerateHandlerParameter> parametersToValidateSync = new();
        List<EndpointToGenerateHandlerParameter> parametersToValidateAsync = new();

        foreach (var parameter in endpoint.Handler.Parameters)
        {
            if (!_validators.TryGetValue(parameter.ToString(), out var validator))
            {
                continue;
            }

            if (parameter is { IsValueType: true, IsNullable: true })
            {
                Diagnostic diagnostic = Diagnostic.Create(DiagnosticDescriptors.NullableValueTypeWillNotBeValidated,
                    parameter.Symbol.Locations[0], parameter);
                Diagnostics.Add(diagnostic);
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

        bool anyFilterAdded = false;

        switch (parametersToValidateSync.Count)
        {
            case 1:
                AddValidationFilter(endpoint.Handler.EndpointParameter, parametersToValidateSync[0]);
                anyFilterAdded = true;
                break;
            case > 1:
                AddValidationFilter(endpoint.Handler.EndpointParameter, parametersToValidateSync);
                anyFilterAdded = true;
                break;
        }

        switch (parametersToValidateAsync.Count)
        {
            case 1:
                AddValidationFilterAsync(endpoint.Handler.EndpointParameter, parametersToValidateAsync[0]);
                anyFilterAdded = true;
                break;
            case > 1:
                AddValidationFilterAsync(endpoint.Handler.EndpointParameter, parametersToValidateAsync);
                anyFilterAdded = true;
                break;
        }

        return anyFilterAdded;
    }

    private void AddValidationFilter(
        EndpointToGenerateHandlerParameter endpoint,
        EndpointToGenerateHandlerParameter parameter)
    {
        using IDisposable filterBlock = OpenAddEndpointFilter();
        AppendLine(GetEndpoint(endpoint));
        AppendLine(GetArgument(parameter, "a"));

        if (parameter.HasCustomBinding)
        {
            AddModelBindingFailed();
        }

        AppendLine(parameter.IsNullable
            ? $"{Fqn.ValidationResult} result = a is null ? {Fqn.SuccessValidationResult} : {GetValidator(parameter)}.Validate(a);"
            : $"{Fqn.ValidationResult} result = {GetValidator(parameter)}.Validate(a);");
        AppendLine(
            $"return result.IsValid ? {Next} : {Fqn.ValueTask}.FromResult<object?>({ValidationFailed("result")});");
    }

    private void AddValidationFilter(
        EndpointToGenerateHandlerParameter endpoint,
        IList<EndpointToGenerateHandlerParameter> parameters)
    {
        using IDisposable filterBlock = OpenAddEndpointFilter();
        AppendLine(GetEndpoint(endpoint));

        bool anyCustomBinding = false;

        for (int i = 0; i < parameters.Count; ++i)
        {
            AppendLine(GetArgument(parameters[i], $"a{i}"));
            anyCustomBinding |= parameters[i].HasCustomBinding;
        }

        if (anyCustomBinding)
        {
            AddModelBindingFailed();
        }

        List<string> validationResults = new(parameters.Count);
        validationResults.AddRange(parameters.Select(static (parameter, i) => parameter.IsNullable
            ? $"a{i} is null ? {Fqn.SuccessValidationResult} : {GetValidator(parameter)}.Validate(a{i})"
            : $"{GetValidator(parameter)}.Validate(a{i})"));

        string isValid = string.Join(" && ", validationResults.Select(static (_, i) => $"results[{i}].IsValid"));

        AppendLine($"{Fqn.ValidationResult}[] results = {{ {string.Join(", ", validationResults)} }};");
        AppendLine($"return {isValid} ? {Next} : {Fqn.ValueTask}.FromResult<object?>({ValidationFailed("results")});");
    }

    private void AddValidationFilterAsync(
        EndpointToGenerateHandlerParameter endpoint,
        EndpointToGenerateHandlerParameter parameter)
    {
        using IDisposable filterBlock = OpenAddEndpointFilterAsync();
        AppendLine(GetEndpoint(endpoint));
        AppendLine(GetArgument(parameter, "a"));

        if (parameter.HasCustomBinding)
        {
            AddModelBindingFailedAsync();
        }

        AppendLine(parameter.IsNullable
            ? $"{Fqn.ValidationResult} result = a is null ? {Fqn.SuccessValidationResult} : await {GetValidator(parameter)}.ValidateAsync(a);"
            : $"{Fqn.ValidationResult} result = await {GetValidator(parameter)}.ValidateAsync(a);");
        AppendLine($"return result.IsValid ? await {Next} : {ValidationFailed("result")};");
    }

    private void AddValidationFilterAsync(
        EndpointToGenerateHandlerParameter endpoint,
        IList<EndpointToGenerateHandlerParameter> parameters)
    {
        using IDisposable filterBlock = OpenAddEndpointFilterAsync();
        AppendLine(GetEndpoint(endpoint));

        bool anyCustomBinding = false;

        for (int i = 0; i < parameters.Count; ++i)
        {
            AppendLine(GetArgument(parameters[i], $"a{i}"));
            anyCustomBinding |= parameters[i].HasCustomBinding;
        }

        if (anyCustomBinding)
        {
            AddModelBindingFailedAsync();
        }

        List<string> tasks = new(parameters.Count);
        tasks.AddRange(parameters.Select(static (parameter, i) => parameter.IsNullable
            ? $"a{i} is null ? {Fqn.SuccessValidationResultTask} : {GetValidator(parameter)}.ValidateAsync(a{i})"
            : $"{GetValidator(parameter)}.ValidateAsync(a{i})"));

        string isValid = string.Join(" && ", tasks.Select(static (_, i) => $"results[{i}].IsValid"));

        AppendLine($"{Fqn.ValidationResult}[] results = await {Fqn.Task}.WhenAll({string.Join(", ", tasks)});");
        AppendLine($"return {isValid} ? await {Next} : {ValidationFailed("results")};");
    }
}
