using Microsoft.CodeAnalysis;
using MinimalApiBuilder.Generator.Common;
using MinimalApiBuilder.Generator.Entities;

namespace MinimalApiBuilder.Generator.CodeGeneration.Builders;

internal sealed partial class Endpoints
{
    private AddValidationResult AddValidation(EndpointToGenerate endpoint)
    {
        List<EndpointToGenerateHandlerParameter> parametersToValidateSync = new();
        List<EndpointToGenerateHandlerParameter> parametersToValidateAsync = new();

        bool anyCustomBinding = false;

        foreach (var parameter in endpoint.Handler.Parameters)
        {
            anyCustomBinding |= parameter.HasCustomBinding;

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
                AddValidationFilter(endpoint, parametersToValidateSync[0]);
                anyFilterAdded = true;
                break;
            case > 1:
                AddValidationFilter(endpoint, parametersToValidateSync);
                anyFilterAdded = true;
                break;
        }

        switch (parametersToValidateAsync.Count)
        {
            case 1:
                AddAsyncValidationFilter(endpoint, parametersToValidateAsync[0]);
                anyFilterAdded = true;
                break;
            case > 1:
                AddAsyncValidationFilter(endpoint, parametersToValidateAsync);
                anyFilterAdded = true;
                break;
        }

        return new AddValidationResult(anyCustomBinding: anyCustomBinding, anyFilterAdded: anyFilterAdded);
    }

    private void AddValidationFilter(
        EndpointToGenerate endpoint,
        EndpointToGenerateHandlerParameter parameter)
    {
        using IDisposable filterBlock = OpenAddEndpointFilter();
        AppendLine(GetArgument(parameter, "a"));

        if (parameter.HasCustomBinding)
        {
            AddModelBindingFailed(endpoint);
        }

        AppendLine(parameter.IsNullable
            ? $"{Fqn.ValidationResult} result = a is null ? {Fqn.SuccessValidationResult} : {GetValidator(parameter)}.Validate(a);"
            : $"{Fqn.ValidationResult} result = {GetValidator(parameter)}.Validate(a);");
        AppendLine(
            $"return result.IsValid ? {Next} : {Fqn.ValueTask}.FromResult<object?>({ValidationFailed("result")});");
    }

    private void AddValidationFilter(
        EndpointToGenerate endpoint,
        IReadOnlyList<EndpointToGenerateHandlerParameter> parameters)
    {
        using IDisposable filterBlock = OpenAddEndpointFilter();

        bool anyCustomBinding = false;

        for (int i = 0; i < parameters.Count; ++i)
        {
            AppendLine(GetArgument(parameters[i], $"a{i}"));
            anyCustomBinding |= parameters[i].HasCustomBinding;
        }

        if (anyCustomBinding)
        {
            AddModelBindingFailed(endpoint);
        }

        List<string> results = new(parameters.Count);
        results.AddRange(parameters.Select(static (parameter, i) => parameter.IsNullable
            ? $"a{i} is null ? {Fqn.SuccessValidationResult} : {GetValidator(parameter)}.Validate(a{i})"
            : $"{GetValidator(parameter)}.Validate(a{i})"));

        string isValid = string.Join(" && ", results.Select(static (_, i) => $"results[{i}].IsValid"));

        AppendLine($"{Fqn.ValidationResult}[] results = {{ {string.Join(", ", results)} }};");
        AppendLine($"return {isValid} ? {Next} : {Fqn.ValueTask}.FromResult<object?>({ValidationFailed("results")});");
    }

    private void AddAsyncValidationFilter(
        EndpointToGenerate endpoint,
        EndpointToGenerateHandlerParameter parameter)
    {
        using IDisposable filterBlock = OpenAddEndpointFilterAsync();
        AppendLine(GetArgument(parameter, "a"));

        if (parameter.HasCustomBinding)
        {
            AddAsyncModelBindingFailed(endpoint);
        }

        AppendLine(parameter.IsNullable
            ? $"{Fqn.ValidationResult} result = a is null ? {Fqn.SuccessValidationResult} : await {GetValidator(parameter)}.ValidateAsync(a);"
            : $"{Fqn.ValidationResult} result = await {GetValidator(parameter)}.ValidateAsync(a);");
        AppendLine($"return result.IsValid ? await {Next} : {ValidationFailed("result")};");
    }

    private void AddAsyncValidationFilter(
        EndpointToGenerate endpoint,
        IReadOnlyList<EndpointToGenerateHandlerParameter> parameters)
    {
        using IDisposable filterBlock = OpenAddEndpointFilterAsync();

        bool anyCustomBinding = false;

        for (int i = 0; i < parameters.Count; ++i)
        {
            AppendLine(GetArgument(parameters[i], $"a{i}"));
            anyCustomBinding |= parameters[i].HasCustomBinding;
        }

        if (anyCustomBinding)
        {
            AddAsyncModelBindingFailed(endpoint);
        }

        List<string> tasks = new(parameters.Count);
        tasks.AddRange(parameters.Select(static (parameter, i) => parameter.IsNullable
            ? $"a{i} is null ? {Fqn.SuccessValidationResultTask} : {GetValidator(parameter)}.ValidateAsync(a{i})"
            : $"{GetValidator(parameter)}.ValidateAsync(a{i})"));

        string isValid = string.Join(" && ", tasks.Select(static (_, i) => $"results[{i}].IsValid"));

        AppendLine($"{Fqn.ValidationResult}[] results = await {Fqn.Task}.WhenAll({string.Join(", ", tasks)});");
        AppendLine($"return {isValid} ? await {Next} : {ValidationFailed("results")};");
    }

    private readonly struct AddValidationResult
    {
        public AddValidationResult(bool anyCustomBinding, bool anyFilterAdded)
        {
            AnyCustomBinding = anyCustomBinding;
            AnyFilterAdded = anyFilterAdded;
        }

        public bool AnyCustomBinding { get; }

        public bool AnyFilterAdded { get; }
    }
}
