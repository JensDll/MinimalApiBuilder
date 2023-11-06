using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MinimalApiBuilder;

public static class MinimalApiBuilderExtensions
{
    public static RouteHandlerBuilder Map<TEndpoint>(this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern)
        where TEndpoint : IEndpoint
    {
        return endpoints.Map(pattern, TEndpoint._auto_generated_Handler).Configure<TEndpoint>();
    }

    public static RouteHandlerBuilder MapMethods<TEndpoint>(this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern, IEnumerable<string> httpMethods)
        where TEndpoint : IEndpoint
    {
        return endpoints.MapMethods(pattern, httpMethods, TEndpoint._auto_generated_Handler).Configure<TEndpoint>();
    }

    public static RouteHandlerBuilder MapGet<TEndpoint>(this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern)
        where TEndpoint : IEndpoint
    {
        return endpoints.MapGet(pattern, TEndpoint._auto_generated_Handler).Configure<TEndpoint>();
    }

    public static RouteHandlerBuilder MapPost<TEndpoint>(this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern)
        where TEndpoint : IEndpoint
    {
        return endpoints.MapPost(pattern, TEndpoint._auto_generated_Handler).Configure<TEndpoint>();
    }

    public static RouteHandlerBuilder MapPut<TEndpoint>(this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern)
        where TEndpoint : IEndpoint
    {
        return endpoints.MapPut(pattern, TEndpoint._auto_generated_Handler).Configure<TEndpoint>();
    }

    public static RouteHandlerBuilder MapPatch<TEndpoint>(this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern)
        where TEndpoint : IEndpoint
    {
        return endpoints.MapPatch(pattern, TEndpoint._auto_generated_Handler).Configure<TEndpoint>();
    }

    public static RouteHandlerBuilder MapDelete<TEndpoint>(this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern)
        where TEndpoint : IEndpoint
    {
        return endpoints.MapDelete(pattern, TEndpoint._auto_generated_Handler).Configure<TEndpoint>();
    }

    private static RouteHandlerBuilder Configure<TEndpoint>(this RouteHandlerBuilder builder)
        where TEndpoint : IEndpoint
    {
        TEndpoint._auto_generated_Configure(builder);
        TEndpoint.Configure(builder);
        return builder;
    }
}
