﻿//HintName: ConfigureEndpoints.Specialization.g.cs
// <auto-generated>
// This is a MinimalApiBuilder source generated file.
// </auto-generated>

#nullable enable

namespace MinimalApiBuilder
{
    internal static partial class ConfigureEndpoints
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
        private static readonly global::System.Collections.Generic.Dictionary<(string, int), global::System.Action<global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder[]>> s_map0 = new()
        {
            [(@"", 35)] = static (global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder[] builders) =>
            {
                global::MyNamespace.Nested.E1._auto_generated_Configure(builders[0]);
                global::MyNamespace.Nested.E1.Configure(builders[0]);
                global::MyNamespace.Nested.E2._auto_generated_Configure(builders[1]);
                global::MyNamespace.Nested.E2.Configure(builders[1]);
            },
            [(@"", 39)] = static (global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder[] builders) =>
            {
                global::MyNamespace.Nested.E1._auto_generated_Configure(builders[0]);
                global::MyNamespace.Nested.E1.Configure(builders[0]);
                global::MyNamespace.Nested.E2._auto_generated_Configure(builders[1]);
                global::MyNamespace.Nested.E2.Configure(builders[1]);
            },
        };
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
        public static void Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder b0, global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder b1, [global::System.Runtime.CompilerServices.CallerFilePathAttribute] string filePath = "", [global::System.Runtime.CompilerServices.CallerLineNumberAttribute] int lineNumber = 0)
        {
            var configure = s_map0[(filePath, lineNumber)];
            configure(new[] { b0, b1 });
        }
    }
}
