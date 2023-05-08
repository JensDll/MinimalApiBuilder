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
        }
        private const string Name = "global::Features.Endpoint1";
    }
}
namespace Features
{
    public partial class Endpoint2 : IEndpoint
    {
        public static Delegate _auto_generated_Handler { get; } = HandleAsync;
        public static void _auto_generated_Configure(RouteHandlerBuilder builder)
        {
            builder.WithName(Name);
        }
        private const string Name = "global::Features.Endpoint2";
    }
}
