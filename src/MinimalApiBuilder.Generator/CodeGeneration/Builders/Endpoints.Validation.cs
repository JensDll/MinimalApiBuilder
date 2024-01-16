using Microsoft.CodeAnalysis;
using MinimalApiBuilder.Generator.Common;
using MinimalApiBuilder.Generator.Entities;

namespace MinimalApiBuilder.Generator.CodeGeneration.Builders;

internal sealed partial class Endpoints
{
    private AddValidationResult AddValidation(EndpointToGenerate endpoint)
    {
        List<EndpointToGenerateHandlerParameter> toValidateSync = new(endpoint.Handler.Parameters.Length);
        List<EndpointToGenerateHandlerParameter> toValidateAsync = new(endpoint.Handler.Parameters.Length);

        bool anyCustomBinding = false;

        foreach (EndpointToGenerateHandlerParameter parameter in endpoint.Handler.Parameters)
        {
            anyCustomBinding |= parameter.HasCustomBinding;

            if (!_validators.TryGetValue(parameter.ToString(), out ValidatorToGenerate? validator))
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
                toValidateAsync.Add(parameter);
            }
            else
            {
                toValidateSync.Add(parameter);
            }
        }

        switch (toValidateSync.Count)
        {
            case 1:
                AddValidationFilter(endpoint, toValidateSync[0]);
                break;
            case > 1:
                AddValidationFilter(endpoint, toValidateSync);
                break;
        }

        switch (toValidateAsync.Count)
        {
            case 1:
                AddAsyncValidationFilter(endpoint, toValidateAsync[0]);
                break;
            case > 1:
                AddAsyncValidationFilter(endpoint, toValidateAsync);
                break;
        }

        return new AddValidationResult(anyCustomBinding: anyCustomBinding,
            anyFilterAdded: toValidateSync.Count > 0 || toValidateAsync.Count > 0);
    }

    private void AddValidationFilter(
        EndpointToGenerate endpoint,
        EndpointToGenerateHandlerParameter parameter)
    {
        using IDisposable _ = OpenAddEndpointFilter();
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
        List<EndpointToGenerateHandlerParameter> parameters)
    {
        using IDisposable _ = OpenAddEndpointFilter();

        bool anyCustomBinding = false;

        string[] isValidChecks = new string[parameters.Count];
        string[] validateCalls = new string[parameters.Count];

        for (int i = 0; i < parameters.Count; ++i)
        {
            AppendLine(GetArgument(parameters[i], $"a{i}"));
            anyCustomBinding |= parameters[i].HasCustomBinding;
            isValidChecks[i] = $"results[{i}].IsValid";
            validateCalls[i] = parameters[i].IsNullable
                ? $"a{i} is null ? {Fqn.SuccessValidationResult} : {GetValidator(parameters[i])}.Validate(a{i})"
                : $"{GetValidator(parameters[i])}.Validate(a{i})";
        }

        string isValid = string.Join(" && ", isValidChecks);
        string results = string.Join(", ", validateCalls);

        if (anyCustomBinding)
        {
            AddModelBindingFailed(endpoint);
        }

        AppendLine($"{Fqn.ValidationResult}[] results = {{ {results} }};");
        AppendLine($"return {isValid} ? {Next} : {Fqn.ValueTask}.FromResult<object?>({ValidationFailed("results")});");
    }

    private void AddAsyncValidationFilter(
        EndpointToGenerate endpoint,
        EndpointToGenerateHandlerParameter parameter)
    {
        using IDisposable _ = OpenAddAsyncEndpointFilter();
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
        List<EndpointToGenerateHandlerParameter> parameters)
    {
        using IDisposable _ = OpenAddAsyncEndpointFilter();

        bool anyCustomBinding = false;

        string[] isValidChecks = new string[parameters.Count];
        string[] validateCalls = new string[parameters.Count];

        for (int i = 0; i < parameters.Count; ++i)
        {
            AppendLine(GetArgument(parameters[i], $"a{i}"));
            anyCustomBinding |= parameters[i].HasCustomBinding;
            isValidChecks[i] = $"results[{i}].IsValid";
            validateCalls[i] = parameters[i].IsNullable
                ? $"a{i} is null ? {Fqn.SuccessValidationResultTask} : {GetValidator(parameters[i])}.ValidateAsync(a{i})"
                : $"{GetValidator(parameters[i])}.ValidateAsync(a{i})";
        }

        string isValid = string.Join(" && ", isValidChecks);
        string tasks = string.Join(", ", validateCalls);

        if (anyCustomBinding)
        {
            AddAsyncModelBindingFailed(endpoint);
        }

        AppendLine($"{Fqn.ValidationResult}[] results = await {Fqn.Task}.WhenAll({tasks});");
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
