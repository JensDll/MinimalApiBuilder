namespace MinimalApiBuilder.Generator.Entities;

internal static class FullyQualifiedNames
{
    public const string Delegate = "global::System.Delegate";

    public const string RouteHandlerBuilder = "global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder";

    public const string Task = "global::System.Threading.Tasks.Task";

    public const string ValueTask = "global::System.Threading.Tasks.ValueTask";

    public const string ValidationResult = "global::FluentValidation.Results.ValidationResult";

    public const string IEndpoint = "global::MinimalApiBuilder.IEndpoint";

    public const string IServiceCollection = "global::Microsoft.Extensions.DependencyInjection.IServiceCollection";

    public const string IValidator = "global::FluentValidation.IValidator";

    public static string LinqAny(string source, string predicate) =>
        $"global::System.Linq.Enumerable.Any({source}, {predicate})";
}
