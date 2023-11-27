﻿//HintName: Endpoints.g.cs
// <auto-generated>
// This is a MinimalApiBuilder source generated file.
// </auto-generated>

#nullable enable

[global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
public partial class E : global::MinimalApiBuilder.IMinimalApiBuilderEndpoint
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static void _auto_generated_Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
    {
        global::Microsoft.AspNetCore.Http.EndpointFilterExtensions.AddEndpointFilter(builder, static (invocationContext, next) =>
        {
            global::R a = invocationContext.GetArgument<global::R>(2);
            global::E endpoint = invocationContext.GetArgument<global::E>(1);
            if (endpoint.HasValidationError)
            {
                return global::System.Threading.Tasks.ValueTask.FromResult<object?>(global::Microsoft.AspNetCore.Http.TypedResults.ValidationProblem(endpoint.ValidationErrors, type: "https://tools.ietf.org/html/rfc9110#section-15.5.1", title: "One or more model binding errors occurred."));
            }
            global::FluentValidation.Results.ValidationResult result = global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<global::FluentValidation.IValidator<global::R>>(invocationContext.HttpContext.RequestServices).Validate(a);
            return result.IsValid ? next(invocationContext) : global::System.Threading.Tasks.ValueTask.FromResult<object?>(global::Microsoft.AspNetCore.Http.TypedResults.ValidationProblem(global::MinimalApiBuilder.StaticHelper.GetErrors(result), type: "https://tools.ietf.org/html/rfc9110#section-15.5.1", title: "One or more validation errors occurred."));
        });
    }
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static void Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
    {
    }
}
