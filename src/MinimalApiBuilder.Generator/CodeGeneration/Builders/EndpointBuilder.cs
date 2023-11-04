using Microsoft.CodeAnalysis;
using MinimalApiBuilder.Generator.Common;
using MinimalApiBuilder.Generator.Entities;

namespace MinimalApiBuilder.Generator.CodeGeneration.Builders;

internal class EndpointBuilder : SourceBuilder
{
    private const string ModelBindingFailed = $"new {Fqn.ErrorDto} {{ StatusCode = {Fqn.HttpStatusCode}.BadRequest, Message = \"Model binding failed\", Errors = endpoint.ValidationErrors }}";
    private const string Next = "next(invocationContext)";

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

            if (parameter is { IsValueType: true, IsNullable: true })
            {
                Diagnostic diagnostic =
                    Diagnostic.Create(DiagnosticDescriptors.NullableValueTypeWillNotBeValidated, parameter.Symbol.Locations[0], parameter);
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

        switch (parametersToValidateSync.Count)
        {
            case 1:
                AddValidationFilter(endpoint.Handler.EndpointParameter, parametersToValidateSync[0]);
                break;
            case > 1:
                AddValidationFilter(endpoint.Handler.EndpointParameter, parametersToValidateSync);
                break;
        }

        switch (parametersToValidateAsync.Count)
        {
            case 1:
                AddAsyncValidationFilter(endpoint.Handler.EndpointParameter, parametersToValidateAsync[0]);
                break;
            case > 1:
                AddAsyncValidationFilter(endpoint.Handler.EndpointParameter, parametersToValidateAsync);
                break;
        }
    }

    private void AddValidationFilter(EndpointToGenerateHandlerParameter endpoint, EndpointToGenerateHandlerParameter parameter)
    {
        const string errors = $"{Fqn.Linq}.Select(r.Errors, static failure => failure.ErrorMessage)";
        const string errorDto = $"new {Fqn.ErrorDto} {{ StatusCode = {Fqn.HttpStatusCode}.BadRequest, Message = \"Validation failed\", Errors = {errors} }}";
        const string badRequest = $"{Fqn.TypedResults}.BadRequest({errorDto})";

        using IDisposable filterBlock = OpenAddEndpointFilter();
        AppendLine(GetEndpoint(endpoint));
        AppendLine(GetArgument(parameter, "a"));

        if (parameter.HasCustomBinding)
        {
            using IDisposable ifBlock = OpenBlock("if (endpoint.HasValidationError)");
            AppendLine($"return {Fqn.ValueTask}.FromResult<object?>({Fqn.TypedResults}.BadRequest({ModelBindingFailed}));");
        }

        AppendLine(parameter.IsNullable
            ? $"{Fqn.ValidationResult} r = a is null ? {Fqn.SuccessValidationResult} : {GetValidator(parameter)}.Validate(a);"
            : $"{Fqn.ValidationResult} r = {GetValidator(parameter)}.Validate(a);");

        AppendLine($"return r.IsValid ? {Next} : {Fqn.ValueTask}.FromResult<object?>({badRequest});");
    }

    private void AddAsyncValidationFilter(EndpointToGenerateHandlerParameter endpoint, EndpointToGenerateHandlerParameter parameter)
    {
        const string errors = $"{Fqn.Linq}.Select(r.Errors, static failure => failure.ErrorMessage)";
        const string errorDto = $"new {Fqn.ErrorDto} {{ StatusCode = {Fqn.HttpStatusCode}.BadRequest, Message = \"Validation failed\", Errors = {errors} }}";
        const string badRequest = $"{Fqn.TypedResults}.BadRequest({errorDto})";

        using IDisposable filterBlock = OpenAddEndpointFilterAsync();
        AppendLine(GetEndpoint(endpoint));
        AppendLine(GetArgument(parameter, "a"));

        if (parameter.HasCustomBinding)
        {
            using IDisposable ifBlock = OpenBlock("if (endpoint.HasValidationError)");
            AppendLine($"return {Fqn.TypedResults}.BadRequest({ModelBindingFailed});");
        }

        AppendLine(parameter.IsNullable
            ? $"{Fqn.ValidationResult} r = a is null ? {Fqn.SuccessValidationResult} : await {GetValidator(parameter)}.ValidateAsync(a);"
            : $"{Fqn.ValidationResult} r = await {GetValidator(parameter)}.ValidateAsync(a);");

        AppendLine($"return r.IsValid ? await {Next} : {badRequest};");
    }

    private void AddValidationFilter(EndpointToGenerateHandlerParameter endpoint, IList<EndpointToGenerateHandlerParameter> parameters)
    {
        using IDisposable filterBlock = OpenAddEndpointFilter();
        AppendLine(GetEndpoint(endpoint));

        bool anyCustomBinding = false;

        for (int i = 0; i < parameters.Count; ++i)
        {
            var parameter = parameters[i];
            AppendLine(GetArgument(parameter, $"a{i}"));
            anyCustomBinding |= parameter.HasCustomBinding;
        }

        if (anyCustomBinding)
        {
            using IDisposable ifBlock = OpenBlock("if (endpoint.HasValidationError)");
            AppendLine($"return {Fqn.ValueTask}.FromResult<object?>({Fqn.TypedResults}.BadRequest({ModelBindingFailed}));");
        }

        List<string> isValidChecks = new(parameters.Count);
        List<string> validationResults = new(parameters.Count);

        for (int i = 0; i < parameters.Count; ++i)
        {
            var parameter = parameters[i];
            string name = $"r{i}";
            AppendLine(parameter.IsNullable
                ? $"{Fqn.ValidationResult} {name} = a{i} is null ? {Fqn.SuccessValidationResult} : {GetValidator(parameter)}.Validate(a{i});"
                : $"{Fqn.ValidationResult} {name} = {GetValidator(parameter)}.Validate(a{i});");
            isValidChecks.Add($"{name}.IsValid");
            validationResults.Add(name);
        }

        string errors = $"{Fqn.Linq}.SelectMany(new[] {{ {string.Join(", ", validationResults)} }}, static result => result.Errors, static (_, failure) => failure.ErrorMessage)";
        string errorDto = $"new {Fqn.ErrorDto} {{ StatusCode = {Fqn.HttpStatusCode}.BadRequest, Message = \"Validation failed\", Errors = {errors} }}";

        AppendLine($"return {string.Join(" && ", isValidChecks)} ? next(invocationContext) : {Fqn.ValueTask}.FromResult<object?>({Fqn.TypedResults}.BadRequest({errorDto}));");
    }

    private void AddAsyncValidationFilter(EndpointToGenerateHandlerParameter endpoint, IList<EndpointToGenerateHandlerParameter> parameters)
    {
        using IDisposable filterBlock = OpenAddEndpointFilterAsync();
        AppendLine(GetEndpoint(endpoint));


        bool anyCustomBinding = false;

        for (int i = 0; i < parameters.Count; ++i)
        {
            var parameter = parameters[i];
            AppendLine(GetArgument(parameter, $"a{i}"));
            anyCustomBinding |= parameter.HasCustomBinding;
        }

        if (anyCustomBinding)
        {
            using IDisposable ifBlock = OpenBlock("if (endpoint.HasValidationError)");
            AppendLine($"return {Fqn.TypedResults}.BadRequest({ModelBindingFailed});");
        }

        List<string> tasks = new(parameters.Count);
        tasks.AddRange(parameters.Select((parameter, i) => parameter.IsNullable
            ? $"a{i} is null ? {Fqn.SuccessValidationResultTask} : {GetValidator(parameter)}.ValidateAsync(a{i})"
            : $"{GetValidator(parameter)}.ValidateAsync(a{i})"));

        AppendLine($"{Fqn.TaskValidationResult}[] tasks = {{ {string.Join(", ", tasks)} }};");
        AppendLine($"{Fqn.ValidationResult}[] results = await {Fqn.Task}.WhenAll(tasks);");

        string isValid = string.Join(" && ", Enumerable.Range(0, tasks.Count).Select(static i => $"results[{i}].IsValid"));
        const string errors = $"{Fqn.Linq}.SelectMany(results, static result => result.Errors, static (_, failure) => failure.ErrorMessage)";
        const string errorDto = $"new {Fqn.ErrorDto} {{ StatusCode = {Fqn.HttpStatusCode}.BadRequest, Message = \"Validation failed\", Errors = {errors} }}";

        AppendLine($"return {isValid} ? await next(invocationContext) : {Fqn.TypedResults}.BadRequest({errorDto});");
    }

    private IDisposable OpenAddEndpointFilter() =>
        OpenBlock($"{Fqn.AddEndpointFilter}(builder, static (invocationContext, next) =>", ");");

    private IDisposable OpenAddEndpointFilterAsync() =>
        OpenBlock($"{Fqn.AddEndpointFilter}(builder, static async (invocationContext, next) =>", ");");

    private static string GetRequiredService(string type) =>
        $"{Fqn.GetRequiredService}<{type}>(invocationContext.HttpContext.RequestServices)";

    private static string GetValidator(EndpointToGenerateHandlerParameter parameter) =>
        $"{GetRequiredService($"{Fqn.IValidator}<{parameter}>")}";

    private static string GetArgument(EndpointToGenerateHandlerParameter parameter, string variableName) =>
        $"{parameter} {variableName} = invocationContext.GetArgument<{parameter}>({parameter.Position});";

    private static string GetEndpoint(EndpointToGenerateHandlerParameter endpoint) => GetArgument(endpoint, nameof(endpoint));
}
