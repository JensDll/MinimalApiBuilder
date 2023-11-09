using MinimalApiBuilder.Generator.Common;
using MinimalApiBuilder.Generator.Entities;

namespace MinimalApiBuilder.Generator.CodeGeneration.Builders;

internal partial class EndpointBuilder
{
    private const string Next = "next(invocationContext)";

    private IDisposable OpenAddEndpointFilter() =>
        OpenBlockExtra(");", $"{Fqn.AddEndpointFilter}(builder, static (invocationContext, next) =>");

    private IDisposable OpenAddEndpointFilterAsync() =>
        OpenBlockExtra(");", $"{Fqn.AddEndpointFilter}(builder, static async (invocationContext, next) =>");

    private static string GetRequiredService(string type) =>
        $"{Fqn.GetRequiredService}<{type}>(invocationContext.HttpContext.RequestServices)";

    private static string GetValidator(EndpointToGenerateHandlerParameter parameter) =>
        $"{GetRequiredService($"{Fqn.IValidator}<{parameter}>")}";

    private static string GetArgument(EndpointToGenerateHandlerParameter parameter, string varName) =>
        $"{parameter} {varName} = invocationContext.GetArgument<{parameter}>({parameter.Position});";

    private static string GetEndpoint(EndpointToGenerateHandlerParameter endpoint) =>
        GetArgument(endpoint, nameof(endpoint));

    private static string ValidationFailed(string varName, string title = "One or more validation errors occurred.") =>
        $"{Fqn.ValidationProblem}({Fqn.GetErrors}({varName}), title: \"{title}\")";

    private static string ModelBindingFailed(string title = "One or more model binding errors occurred.") =>
        $"{Fqn.ValidationProblem}(endpoint.ValidationErrors, title: \"{title}\")";
}
