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
            global::Microsoft.AspNetCore.Builder.RoutingEndpointConventionBuilderExtensions.WithName(builder, Name);
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
        public static void Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder builder)
        {
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
        public const string Name = "global::Features.Endpoint";
    }
}
