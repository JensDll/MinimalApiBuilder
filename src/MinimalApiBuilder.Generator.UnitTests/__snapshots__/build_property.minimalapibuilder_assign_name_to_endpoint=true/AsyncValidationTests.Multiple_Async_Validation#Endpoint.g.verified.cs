﻿//HintName: Endpoint.g.cs
// <auto-generated>
// This is a MinimalApiBuilder source generated file.
// </auto-generated>

#nullable enable

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
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
            builder.WithName(Name);
            builder.AddEndpointFilter(static async (invocationContext, next) =>
            {
                global::Features.Endpoint1 endpoint = invocationContext.GetArgument<global::Features.Endpoint1>(0);
                ValidationResult[] results = await Task.WhenAll(invocationContext.HttpContext.RequestServices.GetRequiredService<IValidator<global::Features.Request>>().ValidateAsync(invocationContext.GetArgument<global::Features.Request>(1)), invocationContext.HttpContext.RequestServices.GetRequiredService<IValidator<global::Features.Request>>().ValidateAsync(invocationContext.GetArgument<global::Features.Request>(2)));
                return results.Any(static result => !result.IsValid) ? IEndpoint.GetErrorResult(endpoint, results) : await next(invocationContext);
            });
        }
        private const string Name = "global::Features.Endpoint1";
    }
}
