using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MinimalApiBuilder;

/// <inheritdoc cref="EndpointRouteBuilderExtensions" />
public static class MinimalApiBuilderExtensions
{
    /// <summary>
    /// Adds a <see cref="RouteEndpoint" /> to the <see cref="IEndpointRouteBuilder" /> that matches HTTP requests
    /// for the specified pattern.
    /// </summary>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder" /> to add the route to.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <typeparam name="TEndpoint">The <see cref="MinimalApiBuilderEndpoint" /> handling the request.</typeparam>
    /// <returns>A <see cref="RouteHandlerBuilder" /> that can be used to further customize the endpoint.</returns>
    public static RouteHandlerBuilder Map<TEndpoint>(this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern)
        where TEndpoint : IMinimalApiBuilderEndpoint
    {
        return endpoints.Map(pattern, TEndpoint._auto_generated_Handler).Configure<TEndpoint>();
    }

    /// <summary>
    /// Adds a <see cref="RouteEndpoint" /> to the <see cref="IEndpointRouteBuilder" /> that matches HTTP requests
    /// for the specified HTTP methods and pattern.
    /// </summary>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder" /> to add the route to.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <param name="httpMethods">HTTP methods that the endpoint will match.</param>
    /// <typeparam name="TEndpoint">The <see cref="MinimalApiBuilderEndpoint" /> handling the request.</typeparam>
    /// <returns>A <see cref="RouteHandlerBuilder" /> that can be used to further customize the endpoint.</returns>
    public static RouteHandlerBuilder MapMethods<TEndpoint>(this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern, IEnumerable<string> httpMethods)
        where TEndpoint : IMinimalApiBuilderEndpoint
    {
        return endpoints.MapMethods(pattern, httpMethods, TEndpoint._auto_generated_Handler).Configure<TEndpoint>();
    }

    /// <summary>
    /// Adds a <see cref="RouteEndpoint" /> to the <see cref="IEndpointRouteBuilder" /> that matches HTTP GET requests
    /// for the specified pattern.
    /// </summary>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder" /> to add the route to.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <typeparam name="TEndpoint">The <see cref="MinimalApiBuilderEndpoint" /> handling the request.</typeparam>
    /// <returns>A <see cref="RouteHandlerBuilder" /> that can be used to further customize the endpoint.</returns>
    public static RouteHandlerBuilder MapGet<TEndpoint>(this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern)
        where TEndpoint : IMinimalApiBuilderEndpoint
    {
        return endpoints.MapGet(pattern, TEndpoint._auto_generated_Handler).Configure<TEndpoint>();
    }

    /// <summary>
    /// Adds a <see cref="RouteEndpoint" /> to the <see cref="IEndpointRouteBuilder" /> that matches HTTP POST requests
    /// for the specified pattern.
    /// </summary>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder" /> to add the route to.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <typeparam name="TEndpoint">The <see cref="MinimalApiBuilderEndpoint" /> handling the request.</typeparam>
    /// <returns>A <see cref="RouteHandlerBuilder" /> that can be used to further customize the endpoint.</returns>
    public static RouteHandlerBuilder MapPost<TEndpoint>(this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern)
        where TEndpoint : IMinimalApiBuilderEndpoint
    {
        return endpoints.MapPost(pattern, TEndpoint._auto_generated_Handler).Configure<TEndpoint>();
    }

    /// <summary>
    /// Adds a <see cref="RouteEndpoint" /> to the <see cref="IEndpointRouteBuilder" /> that matches HTTP PUT requests
    /// for the specified pattern.
    /// </summary>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder" /> to add the route to.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <typeparam name="TEndpoint">The <see cref="MinimalApiBuilderEndpoint" /> handling the request.</typeparam>
    /// <returns>A <see cref="RouteHandlerBuilder" /> that can be used to further customize the endpoint.</returns>
    public static RouteHandlerBuilder MapPut<TEndpoint>(this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern)
        where TEndpoint : IMinimalApiBuilderEndpoint
    {
        return endpoints.MapPut(pattern, TEndpoint._auto_generated_Handler).Configure<TEndpoint>();
    }

    /// <summary>
    /// Adds a <see cref="RouteEndpoint" /> to the <see cref="IEndpointRouteBuilder" /> that matches HTTP PATCH requests
    /// for the specified pattern.
    /// </summary>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder" /> to add the route to.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <typeparam name="TEndpoint">The <see cref="MinimalApiBuilderEndpoint" /> handling the request.</typeparam>
    /// <returns>A <see cref="RouteHandlerBuilder" /> that can be used to further customize the endpoint.</returns>
    public static RouteHandlerBuilder MapPatch<TEndpoint>(this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern)
        where TEndpoint : IMinimalApiBuilderEndpoint
    {
        return endpoints.MapPatch(pattern, TEndpoint._auto_generated_Handler).Configure<TEndpoint>();
    }

    /// <summary>
    /// Adds a <see cref="RouteEndpoint" /> to the <see cref="IEndpointRouteBuilder" /> that matches HTTP DELETE requests
    /// for the specified pattern.
    /// </summary>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder" /> to add the route to.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <typeparam name="TEndpoint">The <see cref="MinimalApiBuilderEndpoint" /> handling the request.</typeparam>
    /// <returns>A <see cref="RouteHandlerBuilder" /> that can be used to further customize the endpoint.</returns>
    public static RouteHandlerBuilder MapDelete<TEndpoint>(this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern)
        where TEndpoint : IMinimalApiBuilderEndpoint
    {
        return endpoints.MapDelete(pattern, TEndpoint._auto_generated_Handler).Configure<TEndpoint>();
    }

    private static RouteHandlerBuilder Configure<TEndpoint>(this RouteHandlerBuilder builder)
        where TEndpoint : IMinimalApiBuilderEndpoint
    {
        TEndpoint._auto_generated_Configure(builder);
        TEndpoint.Configure(builder);
        return builder;
    }
}
