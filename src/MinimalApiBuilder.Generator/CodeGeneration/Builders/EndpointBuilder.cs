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
        Options = Options.GetForTarget(endpoint);

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
        using IDisposable filterBlock = OpenAddEndpointFilter();
        AppendLine(GetEndpoint(endpointParameter));
        AppendLine(GetArgument(parameter, "a"));

        if (parameter.HasCustomBinding)
        {
            string nullCheck = parameter.IsValueType ? " || !a.HasValue" : " || a is null";
            string check = $"endpoint.HasValidationError{(parameter.NeedsNullValidation ? nullCheck : "")}";
            using (OpenBlock($"if ({check})"))
            {
                AppendLine($"return {FromResult($"{Fqn.TypedResults}.BadRequest({ModelBindingFailed})")};");
            }
        }

        AppendLine($"{Fqn.ValidationResult} r = {GetValidator(parameter)}.Validate(a);");

        const string errors = $"{Fqn.Linq}.Select(r.Errors, static failure => failure.ErrorMessage)";
        const string errorDto =
            $"new {Fqn.ErrorDto} {{ StatusCode = {Fqn.HttpStatusCode}.BadRequest, Message = \"Validation failed\", Errors = {errors} }}";
        const string badRequest = $"{Fqn.TypedResults}.BadRequest({errorDto})";

        AppendLine(
            $"return r.IsValid ? next(invocationContext) : {FromResult(badRequest)};");
    }

    private void AddValidatorsFilter(
        EndpointToGenerateHandlerParameter endpointParameter,
        IList<EndpointToGenerateHandlerParameter> parameters)
    {
        using IDisposable filterBlock = OpenAddEndpointFilter();

        AppendLine(GetEndpoint(endpointParameter));

        List<string> nullChecks = new(parameters.Count);

        bool anyCustomBinding = false;

        for (int i = 0; i < parameters.Count; ++i)
        {
            var parameter = parameters[i];
            AppendLine(GetArgument(parameter, $"a{i}"));

            if (parameter.NeedsNullValidation)
            {
                nullChecks.Add(parameter.IsValueType ? $"!a{i}.HasValue" : $"a{i} is null");
            }

            anyCustomBinding |= parameter.HasCustomBinding;
        }

        if (anyCustomBinding)
        {
            nullChecks.Insert(0, "endpoint.HasValidationError");
            using (OpenBlock($"if ({string.Join(" || ", nullChecks)})"))
            {
                AppendLine($"return {FromResult($"{Fqn.TypedResults}.BadRequest({ModelBindingFailed})")};");
            }
        }

        List<string> isValidChecks = new(parameters.Count);
        List<string> validationResults = new(parameters.Count);

        for (int i = 0; i < parameters.Count; ++i)
        {
            var parameter = parameters[i];
            AppendLine($"{Fqn.ValidationResult} r{i} = {GetValidator(parameter)}.Validate(a{i});");
            isValidChecks.Add($"r{i}.IsValid");
            validationResults.Add($"r{i}");
        }

        string errors =
            $"{Fqn.Linq}.SelectMany(new[] {{ {string.Join(", ", validationResults)} }}, static result => result.Errors, static (_, failure) => failure.ErrorMessage)";
        string errorDto =
            $"new {Fqn.ErrorDto} {{ StatusCode = {Fqn.HttpStatusCode}.BadRequest, Message = \"Validation failed\", Errors = {errors} }}";

        AppendLine(
            $"return {string.Join(" && ", isValidChecks)} ? next(invocationContext) : {FromResult($"{Fqn.TypedResults}.BadRequest({errorDto})")};");
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
                $"return result.IsValid ? await next(invocationContext) : {Fqn.IEndpoint}.GetValidationErrorResult(endpoint, result);");
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
                $"return {Fqn.Linq}.Any(results, static result => !result.IsValid) ? {Fqn.IEndpoint}.GetValidationErrorResult(endpoint, results) : await next(invocationContext);");
        }
    }

    private static string GetValidator(EndpointToGenerateHandlerParameter parameter) =>
        $"{GetRequiredService($"{Fqn.IValidator}<{parameter}>")}";

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

    private static string GetArgument(EndpointToGenerateHandlerParameter parameter, string variableName) =>
        $"{parameter} {variableName} = invocationContext.GetArgument<{parameter}>({parameter.Position});";

    private static string GetEndpoint(EndpointToGenerateHandlerParameter endpoint) => GetArgument(endpoint, "endpoint");

    private static string FromResult(string value) => $"{Fqn.ValueTask}.FromResult<object?>({value})";

    private const string ModelBindingFailed =
        $"new {Fqn.ErrorDto} {{ StatusCode = {Fqn.HttpStatusCode}.BadRequest, Message = \"Model binding failed\", Errors = endpoint.ValidationErrors }}";
}
