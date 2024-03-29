﻿//HintName: Endpoints.g.cs
// <auto-generated>
// This is a MinimalApiBuilder source generated file.
// </auto-generated>

#nullable enable

[global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
public partial class E1 : global::MinimalApiBuilder.Generator.IMinimalApiBuilderEndpoint
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static void _auto_generated_Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
    {
        global::Microsoft.AspNetCore.Http.EndpointFilterExtensions.AddEndpointFilter(builder, static (invocationContext, next) =>
        {
            global::R1 a = invocationContext.GetArgument<global::R1>(1);
            global::E1 endpoint = invocationContext.GetArgument<global::E1>(0);
            if (endpoint.HasValidationError)
            {
                return global::System.Threading.Tasks.ValueTask.FromResult<object?>(global::Microsoft.AspNetCore.Http.TypedResults.ValidationProblem(endpoint.ValidationErrors, type: "https://tools.ietf.org/html/rfc9110#section-15.5.1", title: "One or more model binding errors occurred."));
            }
            global::FluentValidation.Results.ValidationResult result = global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<global::FluentValidation.IValidator<global::R1>>(invocationContext.HttpContext.RequestServices).Validate(a);
            return result.IsValid ? next(invocationContext) : global::System.Threading.Tasks.ValueTask.FromResult<object?>(global::Microsoft.AspNetCore.Http.TypedResults.ValidationProblem(global::MinimalApiBuilder.Generator.StaticHelper.GetErrors(result), type: "https://tools.ietf.org/html/rfc9110#section-15.5.1", title: "One or more validation errors occurred."));
        });
    }
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static void Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
    {
    }
}
[global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
public partial class E2 : global::MinimalApiBuilder.Generator.IMinimalApiBuilderEndpoint
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static void _auto_generated_Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
    {
        global::Microsoft.AspNetCore.Http.EndpointFilterExtensions.AddEndpointFilter(builder, static (invocationContext, next) =>
        {
            global::R1 a = invocationContext.GetArgument<global::R1>(1);
            global::E2 endpoint = invocationContext.GetArgument<global::E2>(0);
            if (endpoint.HasValidationError)
            {
                return global::System.Threading.Tasks.ValueTask.FromResult<object?>(global::Microsoft.AspNetCore.Http.TypedResults.ValidationProblem(endpoint.ValidationErrors, type: "https://tools.ietf.org/html/rfc9110#section-15.5.1", title: "One or more model binding errors occurred."));
            }
            global::FluentValidation.Results.ValidationResult result = a is null ? global::MinimalApiBuilder.Generator.StaticHelper.SuccessValidationResult : global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<global::FluentValidation.IValidator<global::R1>>(invocationContext.HttpContext.RequestServices).Validate(a);
            return result.IsValid ? next(invocationContext) : global::System.Threading.Tasks.ValueTask.FromResult<object?>(global::Microsoft.AspNetCore.Http.TypedResults.ValidationProblem(global::MinimalApiBuilder.Generator.StaticHelper.GetErrors(result), type: "https://tools.ietf.org/html/rfc9110#section-15.5.1", title: "One or more validation errors occurred."));
        });
    }
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static void Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
    {
    }
}
[global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
public partial class E3 : global::MinimalApiBuilder.Generator.IMinimalApiBuilderEndpoint
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static void _auto_generated_Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
    {
        global::Microsoft.AspNetCore.Http.EndpointFilterExtensions.AddEndpointFilter(builder, static (invocationContext, next) =>
        {
            global::R1 a = invocationContext.GetArgument<global::R1>(1);
            global::E3 endpoint = invocationContext.GetArgument<global::E3>(0);
            if (endpoint.HasValidationError)
            {
                return global::System.Threading.Tasks.ValueTask.FromResult<object?>(global::Microsoft.AspNetCore.Http.TypedResults.ValidationProblem(endpoint.ValidationErrors, type: "https://tools.ietf.org/html/rfc9110#section-15.5.1", title: "One or more model binding errors occurred."));
            }
            global::FluentValidation.Results.ValidationResult result = global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<global::FluentValidation.IValidator<global::R1>>(invocationContext.HttpContext.RequestServices).Validate(a);
            return result.IsValid ? next(invocationContext) : global::System.Threading.Tasks.ValueTask.FromResult<object?>(global::Microsoft.AspNetCore.Http.TypedResults.ValidationProblem(global::MinimalApiBuilder.Generator.StaticHelper.GetErrors(result), type: "https://tools.ietf.org/html/rfc9110#section-15.5.1", title: "One or more validation errors occurred."));
        });
        global::Microsoft.AspNetCore.Http.EndpointFilterExtensions.AddEndpointFilter(builder, static async (invocationContext, next) =>
        {
            global::R2 a = invocationContext.GetArgument<global::R2>(2);
            global::E3 endpoint = invocationContext.GetArgument<global::E3>(0);
            if (endpoint.HasValidationError)
            {
                return global::Microsoft.AspNetCore.Http.TypedResults.ValidationProblem(endpoint.ValidationErrors, type: "https://tools.ietf.org/html/rfc9110#section-15.5.1", title: "One or more model binding errors occurred.");
            }
            global::FluentValidation.Results.ValidationResult result = await global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<global::FluentValidation.IValidator<global::R2>>(invocationContext.HttpContext.RequestServices).ValidateAsync(a);
            return result.IsValid ? await next(invocationContext) : global::Microsoft.AspNetCore.Http.TypedResults.ValidationProblem(global::MinimalApiBuilder.Generator.StaticHelper.GetErrors(result), type: "https://tools.ietf.org/html/rfc9110#section-15.5.1", title: "One or more validation errors occurred.");
        });
    }
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static void Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
    {
    }
}
[global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
public partial class E4 : global::MinimalApiBuilder.Generator.IMinimalApiBuilderEndpoint
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static void _auto_generated_Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
    {
        global::Microsoft.AspNetCore.Http.EndpointFilterExtensions.AddEndpointFilter(builder, static (invocationContext, next) =>
        {
            global::R1 a = invocationContext.GetArgument<global::R1>(1);
            global::E4 endpoint = invocationContext.GetArgument<global::E4>(0);
            if (endpoint.HasValidationError)
            {
                return global::System.Threading.Tasks.ValueTask.FromResult<object?>(global::Microsoft.AspNetCore.Http.TypedResults.ValidationProblem(endpoint.ValidationErrors, type: "https://tools.ietf.org/html/rfc9110#section-15.5.1", title: "One or more model binding errors occurred."));
            }
            global::FluentValidation.Results.ValidationResult result = a is null ? global::MinimalApiBuilder.Generator.StaticHelper.SuccessValidationResult : global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<global::FluentValidation.IValidator<global::R1>>(invocationContext.HttpContext.RequestServices).Validate(a);
            return result.IsValid ? next(invocationContext) : global::System.Threading.Tasks.ValueTask.FromResult<object?>(global::Microsoft.AspNetCore.Http.TypedResults.ValidationProblem(global::MinimalApiBuilder.Generator.StaticHelper.GetErrors(result), type: "https://tools.ietf.org/html/rfc9110#section-15.5.1", title: "One or more validation errors occurred."));
        });
        global::Microsoft.AspNetCore.Http.EndpointFilterExtensions.AddEndpointFilter(builder, static async (invocationContext, next) =>
        {
            global::R2 a = invocationContext.GetArgument<global::R2>(2);
            global::E4 endpoint = invocationContext.GetArgument<global::E4>(0);
            if (endpoint.HasValidationError)
            {
                return global::Microsoft.AspNetCore.Http.TypedResults.ValidationProblem(endpoint.ValidationErrors, type: "https://tools.ietf.org/html/rfc9110#section-15.5.1", title: "One or more model binding errors occurred.");
            }
            global::FluentValidation.Results.ValidationResult result = await global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<global::FluentValidation.IValidator<global::R2>>(invocationContext.HttpContext.RequestServices).ValidateAsync(a);
            return result.IsValid ? await next(invocationContext) : global::Microsoft.AspNetCore.Http.TypedResults.ValidationProblem(global::MinimalApiBuilder.Generator.StaticHelper.GetErrors(result), type: "https://tools.ietf.org/html/rfc9110#section-15.5.1", title: "One or more validation errors occurred.");
        });
    }
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static void Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
    {
    }
}
[global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
public partial class E5 : global::MinimalApiBuilder.Generator.IMinimalApiBuilderEndpoint
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static void _auto_generated_Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
    {
        global::Microsoft.AspNetCore.Http.EndpointFilterExtensions.AddEndpointFilter(builder, static (invocationContext, next) =>
        {
            global::R1 a = invocationContext.GetArgument<global::R1>(1);
            global::E5 endpoint = invocationContext.GetArgument<global::E5>(0);
            if (endpoint.HasValidationError)
            {
                return global::System.Threading.Tasks.ValueTask.FromResult<object?>(global::Microsoft.AspNetCore.Http.TypedResults.ValidationProblem(endpoint.ValidationErrors, type: "https://tools.ietf.org/html/rfc9110#section-15.5.1", title: "One or more model binding errors occurred."));
            }
            global::FluentValidation.Results.ValidationResult result = a is null ? global::MinimalApiBuilder.Generator.StaticHelper.SuccessValidationResult : global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<global::FluentValidation.IValidator<global::R1>>(invocationContext.HttpContext.RequestServices).Validate(a);
            return result.IsValid ? next(invocationContext) : global::System.Threading.Tasks.ValueTask.FromResult<object?>(global::Microsoft.AspNetCore.Http.TypedResults.ValidationProblem(global::MinimalApiBuilder.Generator.StaticHelper.GetErrors(result), type: "https://tools.ietf.org/html/rfc9110#section-15.5.1", title: "One or more validation errors occurred."));
        });
        global::Microsoft.AspNetCore.Http.EndpointFilterExtensions.AddEndpointFilter(builder, static async (invocationContext, next) =>
        {
            global::R2 a = invocationContext.GetArgument<global::R2>(2);
            global::E5 endpoint = invocationContext.GetArgument<global::E5>(0);
            if (endpoint.HasValidationError)
            {
                return global::Microsoft.AspNetCore.Http.TypedResults.ValidationProblem(endpoint.ValidationErrors, type: "https://tools.ietf.org/html/rfc9110#section-15.5.1", title: "One or more model binding errors occurred.");
            }
            global::FluentValidation.Results.ValidationResult result = a is null ? global::MinimalApiBuilder.Generator.StaticHelper.SuccessValidationResult : await global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<global::FluentValidation.IValidator<global::R2>>(invocationContext.HttpContext.RequestServices).ValidateAsync(a);
            return result.IsValid ? await next(invocationContext) : global::Microsoft.AspNetCore.Http.TypedResults.ValidationProblem(global::MinimalApiBuilder.Generator.StaticHelper.GetErrors(result), type: "https://tools.ietf.org/html/rfc9110#section-15.5.1", title: "One or more validation errors occurred.");
        });
    }
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static void Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
    {
    }
}
