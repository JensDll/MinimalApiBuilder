using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MinimalApiBuilder;

public static class MinimalApiBuilderExtensions
{
    public static RouteHandlerBuilder MapGet<TEndpoint>(this IEndpointRouteBuilder app, string pattern)
        where TEndpoint : IEndpoint
    {
        return app.MapGet(pattern, TEndpoint._auto_generated_Handler).Configure<TEndpoint>();
    }

    public static RouteHandlerBuilder MapPost<TEndpoint>(this IEndpointRouteBuilder app, string pattern)
        where TEndpoint : IEndpoint
    {
        return app.MapPost(pattern, TEndpoint._auto_generated_Handler).Configure<TEndpoint>();
    }

    public static RouteHandlerBuilder MapPut<TEndpoint>(this IEndpointRouteBuilder app, string pattern)
        where TEndpoint : IEndpoint
    {
        return app.MapPut(pattern, TEndpoint._auto_generated_Handler).Configure<TEndpoint>();
    }

    public static RouteHandlerBuilder MapPatch<TEndpoint>(this IEndpointRouteBuilder app, string pattern)
        where TEndpoint : IEndpoint
    {
        return app.MapPatch(pattern, TEndpoint._auto_generated_Handler).Configure<TEndpoint>();
    }

    public static RouteHandlerBuilder MapDelete<TEndpoint>(this IEndpointRouteBuilder app, string pattern)
        where TEndpoint : IEndpoint
    {
        return app.MapDelete(pattern, TEndpoint._auto_generated_Handler).Configure<TEndpoint>();
    }

    private static RouteHandlerBuilder Configure<TEndpoint>(this RouteHandlerBuilder builder)
        where TEndpoint : IEndpoint
    {
        TEndpoint._auto_generated_Configure(builder);
        TEndpoint.Configure(builder);
        return builder;
    }
}
