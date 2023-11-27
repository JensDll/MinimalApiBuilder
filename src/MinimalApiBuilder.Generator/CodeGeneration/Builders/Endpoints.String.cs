using MinimalApiBuilder.Generator.Common;
using MinimalApiBuilder.Generator.Entities;

namespace MinimalApiBuilder.Generator.CodeGeneration.Builders;

internal sealed partial class Endpoints
{
    private const string Next = "next(invocationContext)";

    private IDisposable OpenAddEndpointFilter() =>
        OpenBlockExtra(");", $"{Fqn.AddEndpointFilter}(builder, static (invocationContext, next) =>");

    private IDisposable OpenAddAsyncEndpointFilter() =>
        OpenBlockExtra(");", $"{Fqn.AddEndpointFilter}(builder, static async (invocationContext, next) =>");

    private static string GetRequiredService<T>(T type) =>
        $"{Fqn.GetRequiredService}<{type}>(invocationContext.HttpContext.RequestServices)";

    private static string GetValidator<T>(T type) => $"{GetRequiredService($"{Fqn.IValidator}<{type}>")}";

    private static string GetArgument(EndpointToGenerateHandlerParameter parameter, string name) =>
        $"{parameter} {name} = invocationContext.GetArgument<{parameter}>({parameter.Position});";

    private static string GetEndpoint(EndpointToGenerate endpoint) => endpoint.Handler.EndpointParameter is null
        ? $"{endpoint} {nameof(endpoint)} = {GetRequiredService(endpoint)};"
        : GetArgument(endpoint.Handler.EndpointParameter, nameof(endpoint));

    private string ValidationFailed(string name) =>
        $"{Fqn.ValidationProblem}({Fqn.GetErrors}({name}), type: \"{Options.ValidationProblemType}\", title: \"{Options.ValidationProblemTitle}\")";

    private string ModelBindingFailed() =>
        $"{Fqn.ValidationProblem}(endpoint.ValidationErrors, type: \"{Options.ModelBindingProblemType}\", title: \"{Options.ModelBindingProblemTitle}\")";
}
