﻿//HintName: Endpoints.g.cs
// <auto-generated>
// This is a MinimalApiBuilder source generated file.
// </auto-generated>

#nullable enable

[global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
public partial class E1 : global::MinimalApiBuilder.IMinimalApiBuilderEndpoint
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static void _auto_generated_Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
    {
        global::Microsoft.AspNetCore.Http.EndpointFilterExtensions.AddEndpointFilter(builder, static async (invocationContext, next) =>
        {
            global::E1 endpoint = invocationContext.GetArgument<global::E1>(0);
            global::R1 a = invocationContext.GetArgument<global::R1>(1);
            if (endpoint.HasValidationError)
            {
                return global::Microsoft.AspNetCore.Http.TypedResults.ValidationProblem(endpoint.ValidationErrors, title: "One or more model binding errors occurred.");
            }
            global::FluentValidation.Results.ValidationResult result = await global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<global::FluentValidation.IValidator<global::R1>>(invocationContext.HttpContext.RequestServices).ValidateAsync(a);
            return result.IsValid ? await next(invocationContext) : global::Microsoft.AspNetCore.Http.TypedResults.ValidationProblem(global::MinimalApiBuilder.StaticHelper.GetErrors(result), title: "One or more validation errors occurred.");
        });
    }
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static void Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
    {
    }
}
[global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
public partial class E2 : global::MinimalApiBuilder.IMinimalApiBuilderEndpoint
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static void _auto_generated_Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
    {
        global::Microsoft.AspNetCore.Http.EndpointFilterExtensions.AddEndpointFilter(builder, static async (invocationContext, next) =>
        {
            global::E2 endpoint = invocationContext.GetArgument<global::E2>(0);
            global::R1 a = invocationContext.GetArgument<global::R1>(1);
            if (endpoint.HasValidationError)
            {
                return global::Microsoft.AspNetCore.Http.TypedResults.ValidationProblem(endpoint.ValidationErrors, title: "One or more model binding errors occurred.");
            }
            global::FluentValidation.Results.ValidationResult result = a is null ? global::MinimalApiBuilder.StaticHelper.SuccessValidationResult : await global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<global::FluentValidation.IValidator<global::R1>>(invocationContext.HttpContext.RequestServices).ValidateAsync(a);
            return result.IsValid ? await next(invocationContext) : global::Microsoft.AspNetCore.Http.TypedResults.ValidationProblem(global::MinimalApiBuilder.StaticHelper.GetErrors(result), title: "One or more validation errors occurred.");
        });
    }
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static void Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
    {
    }
}
[global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
public partial class E3 : global::MinimalApiBuilder.IMinimalApiBuilderEndpoint
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static void _auto_generated_Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
    {
        global::Microsoft.AspNetCore.Http.EndpointFilterExtensions.AddEndpointFilter(builder, static async (invocationContext, next) =>
        {
            global::E3 endpoint = invocationContext.GetArgument<global::E3>(0);
            global::R1 a0 = invocationContext.GetArgument<global::R1>(1);
            global::R2 a1 = invocationContext.GetArgument<global::R2>(2);
            if (endpoint.HasValidationError)
            {
                return global::Microsoft.AspNetCore.Http.TypedResults.ValidationProblem(endpoint.ValidationErrors, title: "One or more model binding errors occurred.");
            }
            global::FluentValidation.Results.ValidationResult[] results = await global::System.Threading.Tasks.Task.WhenAll(global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<global::FluentValidation.IValidator<global::R1>>(invocationContext.HttpContext.RequestServices).ValidateAsync(a0), global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<global::FluentValidation.IValidator<global::R2>>(invocationContext.HttpContext.RequestServices).ValidateAsync(a1));
            return results[0].IsValid && results[1].IsValid ? await next(invocationContext) : global::Microsoft.AspNetCore.Http.TypedResults.ValidationProblem(global::MinimalApiBuilder.StaticHelper.GetErrors(results), title: "One or more validation errors occurred.");
        });
    }
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static void Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
    {
    }
}
[global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
public partial class E4 : global::MinimalApiBuilder.IMinimalApiBuilderEndpoint
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static void _auto_generated_Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
    {
        global::Microsoft.AspNetCore.Http.EndpointFilterExtensions.AddEndpointFilter(builder, static async (invocationContext, next) =>
        {
            global::E4 endpoint = invocationContext.GetArgument<global::E4>(0);
            global::R1 a0 = invocationContext.GetArgument<global::R1>(1);
            global::R2 a1 = invocationContext.GetArgument<global::R2>(2);
            if (endpoint.HasValidationError)
            {
                return global::Microsoft.AspNetCore.Http.TypedResults.ValidationProblem(endpoint.ValidationErrors, title: "One or more model binding errors occurred.");
            }
            global::FluentValidation.Results.ValidationResult[] results = await global::System.Threading.Tasks.Task.WhenAll(a0 is null ? global::MinimalApiBuilder.StaticHelper.SuccessValidationResultTask : global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<global::FluentValidation.IValidator<global::R1>>(invocationContext.HttpContext.RequestServices).ValidateAsync(a0), global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<global::FluentValidation.IValidator<global::R2>>(invocationContext.HttpContext.RequestServices).ValidateAsync(a1));
            return results[0].IsValid && results[1].IsValid ? await next(invocationContext) : global::Microsoft.AspNetCore.Http.TypedResults.ValidationProblem(global::MinimalApiBuilder.StaticHelper.GetErrors(results), title: "One or more validation errors occurred.");
        });
    }
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static void Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
    {
    }
}
[global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
public partial class E5 : global::MinimalApiBuilder.IMinimalApiBuilderEndpoint
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static void _auto_generated_Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
    {
        global::Microsoft.AspNetCore.Http.EndpointFilterExtensions.AddEndpointFilter(builder, static async (invocationContext, next) =>
        {
            global::E5 endpoint = invocationContext.GetArgument<global::E5>(0);
            global::R1 a0 = invocationContext.GetArgument<global::R1>(1);
            global::R2 a1 = invocationContext.GetArgument<global::R2>(2);
            if (endpoint.HasValidationError)
            {
                return global::Microsoft.AspNetCore.Http.TypedResults.ValidationProblem(endpoint.ValidationErrors, title: "One or more model binding errors occurred.");
            }
            global::FluentValidation.Results.ValidationResult[] results = await global::System.Threading.Tasks.Task.WhenAll(a0 is null ? global::MinimalApiBuilder.StaticHelper.SuccessValidationResultTask : global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<global::FluentValidation.IValidator<global::R1>>(invocationContext.HttpContext.RequestServices).ValidateAsync(a0), a1 is null ? global::MinimalApiBuilder.StaticHelper.SuccessValidationResultTask : global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<global::FluentValidation.IValidator<global::R2>>(invocationContext.HttpContext.RequestServices).ValidateAsync(a1));
            return results[0].IsValid && results[1].IsValid ? await next(invocationContext) : global::Microsoft.AspNetCore.Http.TypedResults.ValidationProblem(global::MinimalApiBuilder.StaticHelper.GetErrors(results), title: "One or more validation errors occurred.");
        });
    }
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static void Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
    {
    }
}
