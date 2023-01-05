﻿//HintName: Endpoint.generated.cs
#nullable enable

// <auto-generated>
// This is a MinimalApiBuilder source generated generated file.
// </auto-generated>

using MinimalApiBuilder;
using FluentValidation;
using FluentValidation.Results;

namespace Features
{
    public partial class Endpoint1 : IEndpoint
    {
        public static Delegate _auto_generated_Handler { get; } = Handle;
        public static void _auto_generated_Configure(RouteHandlerBuilder builder)
        {
            builder.AddEndpointFilter(static (invocationContext, next) =>
            {
                global::Features.Endpoint1 endpoint = invocationContext.GetArgument<global::Features.Endpoint1>(0);
                ValidationResult result = invocationContext.HttpContext.RequestServices.GetRequiredService<IValidator<global::Features.Request>>().Validate(invocationContext.GetArgument<global::Features.Request>(1));
                return result.IsValid ? next(invocationContext) : ValueTask.FromResult<object?>(IEndpoint.GetErrorResult(endpoint, result));
            });
            builder.AddEndpointFilter(static async (invocationContext, next) =>
            {
                global::Features.Endpoint1 endpoint = invocationContext.GetArgument<global::Features.Endpoint1>(0);
                ValidationResult result = await invocationContext.HttpContext.RequestServices.GetRequiredService<IValidator<global::Features.Parameters>>().ValidateAsync(invocationContext.GetArgument<global::Features.Parameters>(2));
                return result.IsValid ? await next(invocationContext) : IEndpoint.GetErrorResult(endpoint, result);
            });
        }
    }
}
