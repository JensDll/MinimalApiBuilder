using MinimalApiBuilder.Generator.Common;
using MinimalApiBuilder.Generator.Entities;

namespace MinimalApiBuilder.Generator.CodeGeneration.Builders;

internal sealed partial class Endpoints
{
    private const string Next = "next(invocationContext)";

    private IDisposable OpenAddEndpointFilter() =>
        OpenBlockExtra(");", $"{Fqn.AddEndpointFilter}(builder, static (invocationContext, next) =>");

    private IDisposable OpenAddEndpointFilterAsync() =>
        OpenBlockExtra(");", $"{Fqn.AddEndpointFilter}(builder, static async (invocationContext, next) =>");

    private static string GetRequiredService<T>(T type) =>
        $"{Fqn.GetRequiredService}<{type}>(invocationContext.HttpContext.RequestServices)";

    private static string GetValidator<T>(T type) => $"{GetRequiredService($"{Fqn.IValidator}<{type}>")}";

    private static string GetArgument(EndpointToGenerateHandlerParameter parameter, string name) =>
        $"{parameter} {name} = invocationContext.GetArgument<{parameter}>({parameter.Position});";

    private static string GetEndpoint(EndpointToGenerate endpoint) => endpoint.Handler.EndpointParameter is null
        ? $"{endpoint} endpoint = {GetRequiredService(endpoint)};"
        : GetArgument(endpoint.Handler.EndpointParameter, "endpoint");

    private static string ValidationFailed(string name, string title = "One or more validation errors occurred.") =>
        $"{Fqn.ValidationProblem}({Fqn.GetErrors}({name}), title: \"{title}\")";

    private static string ModelBindingFailed(string title = "One or more model binding errors occurred.") =>
        $"{Fqn.ValidationProblem}(endpoint.ValidationErrors, title: \"{title}\")";
}
