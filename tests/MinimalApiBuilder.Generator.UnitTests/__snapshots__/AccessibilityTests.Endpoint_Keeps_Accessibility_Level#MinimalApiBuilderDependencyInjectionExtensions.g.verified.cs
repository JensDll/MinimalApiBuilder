﻿//HintName: MinimalApiBuilderDependencyInjectionExtensions.g.cs
// <auto-generated>
// This is a MinimalApiBuilder source generated file.
// </auto-generated>

#nullable enable

namespace MinimalApiBuilder
{
    /// <summary>
    /// Minimal API builder dependency injection extension methods.
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
    public static class MinimalApiBuilderDependencyInjectionExtensions
    {
        /// <summary>
        /// Adds the necessary types to the <see cref="global::Microsoft.Extensions.DependencyInjection.IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="global::Microsoft.Extensions.DependencyInjection.IServiceCollection"/>.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("MinimalApiBuilder.Generator", "1.0.0.0")]
        public static global::Microsoft.Extensions.DependencyInjection.IServiceCollection AddMinimalApiBuilderEndpoints(this global::Microsoft.Extensions.DependencyInjection.IServiceCollection services)
        {
            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddScoped<global::MyNamespace.E>(services);
            return services;
        }
    }
}
