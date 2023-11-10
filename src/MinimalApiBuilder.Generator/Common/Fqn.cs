namespace MinimalApiBuilder.Generator.Common;

internal static class Fqn
{
    public const string Delegate = "global::System.Delegate";

    public const string RouteHandlerBuilder = "global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder";

    public const string Task = "global::System.Threading.Tasks.Task";

    public const string ValueTask = "global::System.Threading.Tasks.ValueTask";

    public const string ValidationResult = "global::FluentValidation.Results.ValidationResult";

    public const string SuccessValidationResult = "global::MinimalApiBuilder.StaticHelper.SuccessValidationResult";

    public const string SuccessValidationResultTask =
        "global::MinimalApiBuilder.StaticHelper.SuccessValidationResultTask";

    public const string GetRequiredService =
        "global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService";

    public const string AddEndpointFilter =
        "global::Microsoft.AspNetCore.Http.EndpointFilterExtensions.AddEndpointFilter";

    public const string WithName =
        "global::Microsoft.AspNetCore.Builder.RoutingEndpointConventionBuilderExtensions.WithName";

    public const string IMinimalApiBuilderEndpoint = "global::MinimalApiBuilder.IMinimalApiBuilderEndpoint";

    public const string IServiceCollection = "global::Microsoft.Extensions.DependencyInjection.IServiceCollection";

    public const string IValidator = "global::FluentValidation.IValidator";

    public const string TypedResults = "global::Microsoft.AspNetCore.Http.TypedResults";

    public const string ValidationProblem = $"{TypedResults}.ValidationProblem";

    public const string GetErrors = "global::MinimalApiBuilder.StaticHelper.GetErrors";
}
