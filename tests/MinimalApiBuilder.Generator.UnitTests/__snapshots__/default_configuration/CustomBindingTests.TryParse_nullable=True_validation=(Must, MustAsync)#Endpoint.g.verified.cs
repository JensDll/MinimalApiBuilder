﻿//HintName: Endpoint.g.cs
// <auto-generated>
// This is a MinimalApiBuilder source generated file.
// </auto-generated>

#nullable enable

public partial class E1 : global::MinimalApiBuilder.IEndpoint
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static global::System.Delegate _auto_generated_Handler { get; } = Handle;
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static void _auto_generated_Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
    {
        global::Microsoft.AspNetCore.Http.EndpointFilterExtensions.AddEndpointFilter(builder, static (invocationContext, next) =>
        {
            global::E1 endpoint = invocationContext.GetArgument<global::E1>(0);
            global::R1 a = invocationContext.GetArgument<global::R1>(1);
            if (endpoint.HasValidationError)
            {
                return global::System.Threading.Tasks.ValueTask.FromResult<object?>(global::Microsoft.AspNetCore.Http.TypedResults.BadRequest(new global::MinimalApiBuilder.ErrorDto { StatusCode = global::System.Net.HttpStatusCode.BadRequest, Message = "Model binding failed", Errors = endpoint.ValidationErrors }));
            }
            global::FluentValidation.Results.ValidationResult r = global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<global::FluentValidation.IValidator<global::R1>>(invocationContext.HttpContext.RequestServices).Validate(a);
            return r.IsValid ? next(invocationContext) : global::System.Threading.Tasks.ValueTask.FromResult<object?>(global::Microsoft.AspNetCore.Http.TypedResults.BadRequest(new global::MinimalApiBuilder.ErrorDto { StatusCode = global::System.Net.HttpStatusCode.BadRequest, Message = "Validation failed", Errors = global::System.Linq.Enumerable.Select(r.Errors, static failure => failure.ErrorMessage) }));
        });
    }
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static void Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
    {
    }
}
public partial class E2 : global::MinimalApiBuilder.IEndpoint
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static global::System.Delegate _auto_generated_Handler { get; } = Handle;
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static void _auto_generated_Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
    {
        global::Microsoft.AspNetCore.Http.EndpointFilterExtensions.AddEndpointFilter(builder, static (invocationContext, next) =>
        {
            global::E2 endpoint = invocationContext.GetArgument<global::E2>(0);
            global::R1 a = invocationContext.GetArgument<global::R1>(1);
            if (endpoint.HasValidationError)
            {
                return global::System.Threading.Tasks.ValueTask.FromResult<object?>(global::Microsoft.AspNetCore.Http.TypedResults.BadRequest(new global::MinimalApiBuilder.ErrorDto { StatusCode = global::System.Net.HttpStatusCode.BadRequest, Message = "Model binding failed", Errors = endpoint.ValidationErrors }));
            }
            global::FluentValidation.Results.ValidationResult r = a is null ? global::MinimalApiBuilder.StaticData.SuccessValidationResult : global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<global::FluentValidation.IValidator<global::R1>>(invocationContext.HttpContext.RequestServices).Validate(a);
            return r.IsValid ? next(invocationContext) : global::System.Threading.Tasks.ValueTask.FromResult<object?>(global::Microsoft.AspNetCore.Http.TypedResults.BadRequest(new global::MinimalApiBuilder.ErrorDto { StatusCode = global::System.Net.HttpStatusCode.BadRequest, Message = "Validation failed", Errors = global::System.Linq.Enumerable.Select(r.Errors, static failure => failure.ErrorMessage) }));
        });
    }
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static void Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
    {
    }
}
public partial class E3 : global::MinimalApiBuilder.IEndpoint
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static global::System.Delegate _auto_generated_Handler { get; } = Handle;
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static void _auto_generated_Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
    {
        global::Microsoft.AspNetCore.Http.EndpointFilterExtensions.AddEndpointFilter(builder, static (invocationContext, next) =>
        {
            global::E3 endpoint = invocationContext.GetArgument<global::E3>(0);
            global::R1 a = invocationContext.GetArgument<global::R1>(1);
            if (endpoint.HasValidationError)
            {
                return global::System.Threading.Tasks.ValueTask.FromResult<object?>(global::Microsoft.AspNetCore.Http.TypedResults.BadRequest(new global::MinimalApiBuilder.ErrorDto { StatusCode = global::System.Net.HttpStatusCode.BadRequest, Message = "Model binding failed", Errors = endpoint.ValidationErrors }));
            }
            global::FluentValidation.Results.ValidationResult r = global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<global::FluentValidation.IValidator<global::R1>>(invocationContext.HttpContext.RequestServices).Validate(a);
            return r.IsValid ? next(invocationContext) : global::System.Threading.Tasks.ValueTask.FromResult<object?>(global::Microsoft.AspNetCore.Http.TypedResults.BadRequest(new global::MinimalApiBuilder.ErrorDto { StatusCode = global::System.Net.HttpStatusCode.BadRequest, Message = "Validation failed", Errors = global::System.Linq.Enumerable.Select(r.Errors, static failure => failure.ErrorMessage) }));
        });
        global::Microsoft.AspNetCore.Http.EndpointFilterExtensions.AddEndpointFilter(builder, static async (invocationContext, next) =>
        {
            global::E3 endpoint = invocationContext.GetArgument<global::E3>(0);
            global::R2 a = invocationContext.GetArgument<global::R2>(2);
            global::FluentValidation.Results.ValidationResult r = await global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<global::FluentValidation.IValidator<global::R2>>(invocationContext.HttpContext.RequestServices).ValidateAsync(a);
            return r.IsValid ? await next(invocationContext) : global::Microsoft.AspNetCore.Http.TypedResults.BadRequest(new global::MinimalApiBuilder.ErrorDto { StatusCode = global::System.Net.HttpStatusCode.BadRequest, Message = "Validation failed", Errors = global::System.Linq.Enumerable.Select(r.Errors, static failure => failure.ErrorMessage) });
        });
    }
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static void Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
    {
    }
}
public partial class E4 : global::MinimalApiBuilder.IEndpoint
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static global::System.Delegate _auto_generated_Handler { get; } = Handle;
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static void _auto_generated_Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
    {
        global::Microsoft.AspNetCore.Http.EndpointFilterExtensions.AddEndpointFilter(builder, static (invocationContext, next) =>
        {
            global::E4 endpoint = invocationContext.GetArgument<global::E4>(0);
            global::R1 a = invocationContext.GetArgument<global::R1>(1);
            if (endpoint.HasValidationError)
            {
                return global::System.Threading.Tasks.ValueTask.FromResult<object?>(global::Microsoft.AspNetCore.Http.TypedResults.BadRequest(new global::MinimalApiBuilder.ErrorDto { StatusCode = global::System.Net.HttpStatusCode.BadRequest, Message = "Model binding failed", Errors = endpoint.ValidationErrors }));
            }
            global::FluentValidation.Results.ValidationResult r = a is null ? global::MinimalApiBuilder.StaticData.SuccessValidationResult : global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<global::FluentValidation.IValidator<global::R1>>(invocationContext.HttpContext.RequestServices).Validate(a);
            return r.IsValid ? next(invocationContext) : global::System.Threading.Tasks.ValueTask.FromResult<object?>(global::Microsoft.AspNetCore.Http.TypedResults.BadRequest(new global::MinimalApiBuilder.ErrorDto { StatusCode = global::System.Net.HttpStatusCode.BadRequest, Message = "Validation failed", Errors = global::System.Linq.Enumerable.Select(r.Errors, static failure => failure.ErrorMessage) }));
        });
        global::Microsoft.AspNetCore.Http.EndpointFilterExtensions.AddEndpointFilter(builder, static async (invocationContext, next) =>
        {
            global::E4 endpoint = invocationContext.GetArgument<global::E4>(0);
            global::R2 a = invocationContext.GetArgument<global::R2>(2);
            global::FluentValidation.Results.ValidationResult r = await global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<global::FluentValidation.IValidator<global::R2>>(invocationContext.HttpContext.RequestServices).ValidateAsync(a);
            return r.IsValid ? await next(invocationContext) : global::Microsoft.AspNetCore.Http.TypedResults.BadRequest(new global::MinimalApiBuilder.ErrorDto { StatusCode = global::System.Net.HttpStatusCode.BadRequest, Message = "Validation failed", Errors = global::System.Linq.Enumerable.Select(r.Errors, static failure => failure.ErrorMessage) });
        });
    }
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static void Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
    {
    }
}
public partial class E5 : global::MinimalApiBuilder.IEndpoint
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static global::System.Delegate _auto_generated_Handler { get; } = Handle;
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static void _auto_generated_Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
    {
        global::Microsoft.AspNetCore.Http.EndpointFilterExtensions.AddEndpointFilter(builder, static (invocationContext, next) =>
        {
            global::E5 endpoint = invocationContext.GetArgument<global::E5>(0);
            global::R1 a = invocationContext.GetArgument<global::R1>(1);
            if (endpoint.HasValidationError)
            {
                return global::System.Threading.Tasks.ValueTask.FromResult<object?>(global::Microsoft.AspNetCore.Http.TypedResults.BadRequest(new global::MinimalApiBuilder.ErrorDto { StatusCode = global::System.Net.HttpStatusCode.BadRequest, Message = "Model binding failed", Errors = endpoint.ValidationErrors }));
            }
            global::FluentValidation.Results.ValidationResult r = a is null ? global::MinimalApiBuilder.StaticData.SuccessValidationResult : global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<global::FluentValidation.IValidator<global::R1>>(invocationContext.HttpContext.RequestServices).Validate(a);
            return r.IsValid ? next(invocationContext) : global::System.Threading.Tasks.ValueTask.FromResult<object?>(global::Microsoft.AspNetCore.Http.TypedResults.BadRequest(new global::MinimalApiBuilder.ErrorDto { StatusCode = global::System.Net.HttpStatusCode.BadRequest, Message = "Validation failed", Errors = global::System.Linq.Enumerable.Select(r.Errors, static failure => failure.ErrorMessage) }));
        });
        global::Microsoft.AspNetCore.Http.EndpointFilterExtensions.AddEndpointFilter(builder, static async (invocationContext, next) =>
        {
            global::E5 endpoint = invocationContext.GetArgument<global::E5>(0);
            global::R2 a = invocationContext.GetArgument<global::R2>(2);
            global::FluentValidation.Results.ValidationResult r = a is null ? global::MinimalApiBuilder.StaticData.SuccessValidationResult : await global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<global::FluentValidation.IValidator<global::R2>>(invocationContext.HttpContext.RequestServices).ValidateAsync(a);
            return r.IsValid ? await next(invocationContext) : global::Microsoft.AspNetCore.Http.TypedResults.BadRequest(new global::MinimalApiBuilder.ErrorDto { StatusCode = global::System.Net.HttpStatusCode.BadRequest, Message = "Validation failed", Errors = global::System.Linq.Enumerable.Select(r.Errors, static failure => failure.ErrorMessage) });
        });
    }
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static void Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
    {
    }
}