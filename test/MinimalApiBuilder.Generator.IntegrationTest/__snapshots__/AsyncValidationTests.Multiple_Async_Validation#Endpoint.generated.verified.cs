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
            builder.AddEndpointFilter(static async (invocationContext, next) =>
            {
                global::Features.Endpoint1 endpoint = invocationContext.GetArgument<global::Features.Endpoint1>(0);
                ValidationResult[] results = await Task.WhenAll(invocationContext.HttpContext.RequestServices.GetRequiredService<IValidator<global::Features.Request>>().ValidateAsync(invocationContext.GetArgument<global::Features.Request>(1)), invocationContext.HttpContext.RequestServices.GetRequiredService<IValidator<global::Features.Request>>().ValidateAsync(invocationContext.GetArgument<global::Features.Request>(2)));
                return results.Any(static result => !result.IsValid) ? IEndpoint.GetErrorResult(endpoint, results) : await next(invocationContext);
            });
        }
    }
}
