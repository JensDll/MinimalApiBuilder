﻿//HintName: Endpoint.g.cs
// <auto-generated>
// This is a MinimalApiBuilder source generated file.
// </auto-generated>

#nullable enable

namespace Features
{
    public partial class Endpoint : global::MinimalApiBuilder.IEndpoint
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
        public static global::System.Delegate _auto_generated_Handler { get; } = Handle;
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
        public static void _auto_generated_Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
        {
            global::Microsoft.AspNetCore.Http.EndpointFilterExtensions.AddEndpointFilter(builder, static (invocationContext, next) =>
            {
                global::Features.Endpoint endpoint = invocationContext.GetArgument<global::Features.Endpoint>(0);
                global::FluentValidation.Results.ValidationResult result = global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<global::FluentValidation.IValidator<global::Features.Request>>(invocationContext.HttpContext.RequestServices).Validate(invocationContext.GetArgument<global::Features.Request>(1));
                return result.IsValid ? next(invocationContext) : global::System.Threading.Tasks.ValueTask.FromResult<object?>(global::MinimalApiBuilder.IEndpoint.GetValidationErrorResult(endpoint, result));
            });
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
        public static void Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
        {
        }
    }
}
