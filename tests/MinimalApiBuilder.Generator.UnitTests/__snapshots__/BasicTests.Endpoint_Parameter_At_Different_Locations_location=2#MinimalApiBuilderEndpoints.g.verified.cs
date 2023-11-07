﻿//HintName: MinimalApiBuilderEndpoints.g.cs
// <auto-generated>
// This is a MinimalApiBuilder source generated file.
// </auto-generated>

#nullable enable

public partial class E : global::MinimalApiBuilder.IMinimalApiBuilderEndpoint
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static global::System.Delegate _auto_generated_Handler { get; } = Handle;
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static void _auto_generated_Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
    {
        global::Microsoft.AspNetCore.Http.EndpointFilterExtensions.AddEndpointFilter(builder, static (invocationContext, next) =>
        {
            global::E endpoint = invocationContext.GetArgument<global::E>(2);
            global::R a = invocationContext.GetArgument<global::R>(1);
            global::FluentValidation.Results.ValidationResult r = global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<global::FluentValidation.IValidator<global::R>>(invocationContext.HttpContext.RequestServices).Validate(a);
            return r.IsValid ? next(invocationContext) : global::System.Threading.Tasks.ValueTask.FromResult<object?>(global::Microsoft.AspNetCore.Http.TypedResults.BadRequest(new global::MinimalApiBuilder.ErrorDto { StatusCode = global::System.Net.HttpStatusCode.BadRequest, Message = "Validation failed", Errors = global::System.Linq.Enumerable.Select(r.Errors, static failure => failure.ErrorMessage) }));
        });
    }
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static void Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
    {
    }
}
