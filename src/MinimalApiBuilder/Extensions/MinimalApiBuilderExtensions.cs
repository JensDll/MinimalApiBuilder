using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MinimalApiBuilder;

/// <inheritdoc cref="EndpointRouteBuilderExtensions" />
public static class MinimalApiBuilderExtensions
{
    /// <summary>
    ///     <inheritdoc cref="EndpointRouteBuilderExtensions.Map(IEndpointRouteBuilder,string,Delegate)" path="/summary" />
    /// </summary>
    /// <param name="endpoints">
    ///     <inheritdoc cref="EndpointRouteBuilderExtensions.Map(IEndpointRouteBuilder,string,Delegate)" path="/param[@name='endpoints']" />
    /// </param>
    /// <param name="pattern">
    ///     <inheritdoc cref="EndpointRouteBuilderExtensions.Map(IEndpointRouteBuilder,string,Delegate)" path="/param[@name='pattern']" />
    /// </param>
    /// <typeparam name="TEndpoint">
    /// The <see cref="MinimalApiBuilderEndpoint" /> handling the request.
    /// </typeparam>
    /// <returns>
    ///     <inheritdoc cref="EndpointRouteBuilderExtensions.Map(IEndpointRouteBuilder,string,Delegate)" path="/returns" />
    /// </returns>
    public static RouteHandlerBuilder Map<TEndpoint>(this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern)
        where TEndpoint : IMinimalApiBuilderEndpoint
    {
        return endpoints.Map(pattern, TEndpoint._auto_generated_Handler).Configure<TEndpoint>();
    }

    /// <summary>
    ///     <inheritdoc cref="EndpointRouteBuilderExtensions.MapMethods(IEndpointRouteBuilder,string,IEnumerable{string},Delegate)" path="/summary" />
    /// </summary>
    /// <param name="endpoints">
    ///     <inheritdoc cref="EndpointRouteBuilderExtensions.MapMethods(IEndpointRouteBuilder,string,IEnumerable{string},Delegate)" path="/param[@name='endpoints']" />
    /// </param>
    /// <param name="pattern">
    ///     <inheritdoc cref="EndpointRouteBuilderExtensions.MapMethods(IEndpointRouteBuilder,string,IEnumerable{string},Delegate)" path="/param[@name='pattern']" />
    /// </param>
    /// <param name="httpMethods">
    ///     <inheritdoc cref="EndpointRouteBuilderExtensions.MapMethods(IEndpointRouteBuilder,string,IEnumerable{string},Delegate)" path="/param[@name='httpMethods']" />
    /// </param>
    /// <typeparam name="TEndpoint">
    /// The <see cref="MinimalApiBuilderEndpoint" /> handling the request.
    /// </typeparam>
    /// <returns>
    ///     <inheritdoc cref="EndpointRouteBuilderExtensions.Map(IEndpointRouteBuilder,string,Delegate)" path="/returns" />
    /// </returns>
    public static RouteHandlerBuilder MapMethods<TEndpoint>(this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern, IEnumerable<string> httpMethods)
        where TEndpoint : IMinimalApiBuilderEndpoint
    {
        return endpoints.MapMethods(pattern, httpMethods, TEndpoint._auto_generated_Handler).Configure<TEndpoint>();
    }

    /// <summary>
    ///     <inheritdoc cref="EndpointRouteBuilderExtensions.MapGet(IEndpointRouteBuilder,string,Delegate)" path="/summary" />
    /// </summary>
    /// <param name="endpoints">
    ///     <inheritdoc cref="EndpointRouteBuilderExtensions.MapGet(IEndpointRouteBuilder,string,Delegate)" path="/param[@name='endpoints']" />
    /// </param>
    /// <param name="pattern">
    ///     <inheritdoc cref="EndpointRouteBuilderExtensions.MapGet(IEndpointRouteBuilder,string,Delegate)" path="/param[@name='pattern']" />
    /// </param>
    /// <typeparam name="TEndpoint">
    /// The <see cref="MinimalApiBuilderEndpoint" /> handling the request.
    /// </typeparam>
    /// <returns>
    ///     <inheritdoc cref="EndpointRouteBuilderExtensions.MapGet(IEndpointRouteBuilder,string,Delegate)" path="/returns" />
    /// </returns>
    public static RouteHandlerBuilder MapGet<TEndpoint>(this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern)
        where TEndpoint : IMinimalApiBuilderEndpoint
    {
        return endpoints.MapGet(pattern, TEndpoint._auto_generated_Handler).Configure<TEndpoint>();
    }

    /// <summary>
    ///     <inheritdoc cref="EndpointRouteBuilderExtensions.MapPost(IEndpointRouteBuilder,string,Delegate)" path="/summary" />
    /// </summary>
    /// <param name="endpoints">
    ///     <inheritdoc cref="EndpointRouteBuilderExtensions.MapPost(IEndpointRouteBuilder,string,Delegate)" path="/param[@name='endpoints']" />
    /// </param>
    /// <param name="pattern">
    ///     <inheritdoc cref="EndpointRouteBuilderExtensions.MapPost(IEndpointRouteBuilder,string,Delegate)" path="/param[@name='pattern']" />
    /// </param>
    /// <typeparam name="TEndpoint">
    /// The <see cref="MinimalApiBuilderEndpoint" /> handling the request.
    /// </typeparam>
    /// <returns>
    ///     <inheritdoc cref="EndpointRouteBuilderExtensions.MapPost(IEndpointRouteBuilder,string,Delegate)" path="/returns" />
    /// </returns>
    public static RouteHandlerBuilder MapPost<TEndpoint>(this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern)
        where TEndpoint : IMinimalApiBuilderEndpoint
    {
        return endpoints.MapPost(pattern, TEndpoint._auto_generated_Handler).Configure<TEndpoint>();
    }

    /// <summary>
    ///     <inheritdoc cref="EndpointRouteBuilderExtensions.MapPut(IEndpointRouteBuilder,string,Delegate)" path="/summary" />
    /// </summary>
    /// <param name="endpoints">
    ///     <inheritdoc cref="EndpointRouteBuilderExtensions.MapPut(IEndpointRouteBuilder,string,Delegate)" path="/param[@name='endpoints']" />
    /// </param>
    /// <param name="pattern">
    ///     <inheritdoc cref="EndpointRouteBuilderExtensions.MapPut(IEndpointRouteBuilder,string,Delegate)" path="/param[@name='pattern']" />
    /// </param>
    /// <typeparam name="TEndpoint">
    /// The <see cref="MinimalApiBuilderEndpoint" /> handling the request.
    /// </typeparam>
    /// <returns>
    ///     <inheritdoc cref="EndpointRouteBuilderExtensions.MapPut(IEndpointRouteBuilder,string,Delegate)" path="/returns" />
    /// </returns>
    public static RouteHandlerBuilder MapPut<TEndpoint>(this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern)
        where TEndpoint : IMinimalApiBuilderEndpoint
    {
        return endpoints.MapPut(pattern, TEndpoint._auto_generated_Handler).Configure<TEndpoint>();
    }

    /// <summary>
    ///     <inheritdoc cref="EndpointRouteBuilderExtensions.MapPatch(IEndpointRouteBuilder,string,Delegate)" path="/summary" />
    /// </summary>
    /// <param name="endpoints">
    ///     <inheritdoc cref="EndpointRouteBuilderExtensions.MapPatch(IEndpointRouteBuilder,string,Delegate)" path="/param[@name='endpoints']" />
    /// </param>
    /// <param name="pattern">
    ///     <inheritdoc cref="EndpointRouteBuilderExtensions.MapPatch(IEndpointRouteBuilder,string,Delegate)" path="/param[@name='pattern']" />
    /// </param>
    /// <typeparam name="TEndpoint">
    /// The <see cref="MinimalApiBuilderEndpoint" /> handling the request.
    /// </typeparam>
    /// <returns>
    ///     <inheritdoc cref="EndpointRouteBuilderExtensions.MapPatch(IEndpointRouteBuilder,string,Delegate)" path="/returns" />
    /// </returns>
    public static RouteHandlerBuilder MapPatch<TEndpoint>(this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern)
        where TEndpoint : IMinimalApiBuilderEndpoint
    {
        return endpoints.MapPatch(pattern, TEndpoint._auto_generated_Handler).Configure<TEndpoint>();
    }

    /// <summary>
    ///     <inheritdoc cref="EndpointRouteBuilderExtensions.MapDelete(IEndpointRouteBuilder,string,Delegate)" path="/summary" />
    /// </summary>
    /// <param name="endpoints">
    ///     <inheritdoc cref="EndpointRouteBuilderExtensions.MapDelete(IEndpointRouteBuilder,string,Delegate)" path="/param[@name='endpoints']" />
    /// </param>
    /// <param name="pattern">
    ///     <inheritdoc cref="EndpointRouteBuilderExtensions.MapDelete(IEndpointRouteBuilder,string,Delegate)" path="/param[@name='pattern']" />
    /// </param>
    /// <typeparam name="TEndpoint">
    /// The <see cref="MinimalApiBuilderEndpoint" /> handling the request.
    /// </typeparam>
    /// <returns>
    ///     <inheritdoc cref="EndpointRouteBuilderExtensions.MapDelete(IEndpointRouteBuilder,string,Delegate)" path="/returns" />
    /// </returns>
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
