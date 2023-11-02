namespace MinimalApiBuilder.Generator.Common;

internal static class Fqn
{
    public const string Delegate = "global::System.Delegate";

    public const string RouteHandlerBuilder = "global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder";

    public const string Task = "global::System.Threading.Tasks.Task";

    public const string ValueTask = "global::System.Threading.Tasks.ValueTask";

    public const string ValidationResult = "global::FluentValidation.Results.ValidationResult";

    public const string Linq = "global::System.Linq.Enumerable";

    public const string GetRequiredService =
        "global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService";

    public const string AddEndpointFilter =
        "global::Microsoft.AspNetCore.Http.EndpointFilterExtensions.AddEndpointFilter";

    public const string WithName =
        "global::Microsoft.AspNetCore.Builder.RoutingEndpointConventionBuilderExtensions.WithName";

    public const string IEndpoint = "global::MinimalApiBuilder.IEndpoint";

    public const string IServiceCollection = "global::Microsoft.Extensions.DependencyInjection.IServiceCollection";

    public const string IValidator = "global::FluentValidation.IValidator";

    public const string HttpStatusCode = "global::System.Net.HttpStatusCode";

    public const string ErrorDto = "global::MinimalApiBuilder.ErrorDto";

    public const string TypedResults = "global::Microsoft.AspNetCore.Http.TypedResults";
}
