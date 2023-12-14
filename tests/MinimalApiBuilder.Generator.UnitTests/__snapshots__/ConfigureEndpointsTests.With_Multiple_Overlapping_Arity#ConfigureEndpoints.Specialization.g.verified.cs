﻿//HintName: ConfigureEndpoints.Specialization.g.cs
// <auto-generated>
// This is a MinimalApiBuilder source generated file.
// </auto-generated>

#nullable enable

namespace MinimalApiBuilder.Generator
{
    internal static partial class ConfigureEndpoints
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
        private static readonly global::System.Collections.Generic.Dictionary<(string, int), global::System.Action<global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder[]>> s_map0 = new()
        {
            [(@"", 44)] = static (global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder[] builders) =>
            {
                global::E1._auto_generated_Configure(builders[0]);
                global::E1.Configure(builders[0]);
                global::E2._auto_generated_Configure(builders[1]);
                global::E2.Configure(builders[1]);
            },
            [(@"", 48)] = static (global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder[] builders) =>
            {
                global::E1._auto_generated_Configure(builders[0]);
                global::E1.Configure(builders[0]);
                global::E2._auto_generated_Configure(builders[1]);
                global::E2.Configure(builders[1]);
            },
        };
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
        public static void Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder b0, global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder b1, [global::System.Runtime.CompilerServices.CallerFilePathAttribute] string filePath = "", [global::System.Runtime.CompilerServices.CallerLineNumberAttribute] int lineNumber = 0)
        {
            var configure = s_map0[(filePath, lineNumber)];
            configure(new[] { b0, b1 });
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
        private static readonly global::System.Collections.Generic.Dictionary<(string, int), global::System.Action<global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder[]>> s_map1 = new()
        {
            [(@"", 52)] = static (global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder[] builders) =>
            {
                global::E3._auto_generated_Configure(builders[0]);
                global::E3.Configure(builders[0]);
            },
            [(@"", 55)] = static (global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder[] builders) =>
            {
                global::E4._auto_generated_Configure(builders[0]);
                global::E4.Configure(builders[0]);
            },
        };
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
        public static void Configure(global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder b0, [global::System.Runtime.CompilerServices.CallerFilePathAttribute] string filePath = "", [global::System.Runtime.CompilerServices.CallerLineNumberAttribute] int lineNumber = 0)
        {
            var configure = s_map1[(filePath, lineNumber)];
            configure(new[] { b0 });
        }
    }
}
