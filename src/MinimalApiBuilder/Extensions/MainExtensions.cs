using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace MinimalApiBuilder;

public static class MainExtensions
{
    public static RouteHandlerBuilder MapGet<TEndpoint>(this IEndpointRouteBuilder app, string pattern)
        where TEndpoint : EndpointBase, IEndpointHandler, IEndpoint
    {
        return app.MapPost(pattern, TEndpoint.Handler)
            .Configure<TEndpoint>(app.ServiceProvider, HttpVerbs.Post);
    }

    public static RouteHandlerBuilder MapPost<TEndpoint>(this IEndpointRouteBuilder app, string pattern)
        where TEndpoint : EndpointBase, IEndpointHandler, IEndpoint
    {
        return app.MapPost(pattern, TEndpoint.Handler)
            .Configure<TEndpoint>(app.ServiceProvider, HttpVerbs.Post);
    }

    public static RouteHandlerBuilder MapPut<TEndpoint>(this IEndpointRouteBuilder app, string pattern)
        where TEndpoint : EndpointBase, IEndpointHandler, IEndpoint
    {
        return app.MapPost(pattern, TEndpoint.Handler)
            .Configure<TEndpoint>(app.ServiceProvider, HttpVerbs.Post);
    }

    public static RouteHandlerBuilder MapPatch<TEndpoint>(this IEndpointRouteBuilder app, string pattern)
        where TEndpoint : EndpointBase, IEndpointHandler, IEndpoint
    {
        return app.MapPost(pattern, TEndpoint.Handler)
            .Configure<TEndpoint>(app.ServiceProvider, HttpVerbs.Post);
    }

    public static RouteHandlerBuilder MapDelete<TEndpoint>(this IEndpointRouteBuilder app, string pattern)
        where TEndpoint : EndpointBase, IEndpointHandler, IEndpoint
    {
        return app.MapPost(pattern, TEndpoint.Handler)
            .Configure<TEndpoint>(app.ServiceProvider, HttpVerbs.Post);
    }

    private static RouteHandlerBuilder Configure<TEndpoint>(
        this RouteHandlerBuilder builder,
        IServiceProvider serviceProvider, HttpVerbs verb)
        where TEndpoint : EndpointBase, IEndpoint
    {
        EndpointConfiguration configuration = new()
        {
            Verb = verb
        };

        int endpointArgumentIndex = TEndpoint.ArgumentPositions[typeof(TEndpoint)];

        builder.AddEndpointFilter((invocationContext, next) =>
        {
            TEndpoint endpoint = invocationContext.GetArgument<TEndpoint>(endpointArgumentIndex);
            endpoint.Configuration = configuration;
            return next(invocationContext);
        });

        using var scope = serviceProvider.CreateScope();

        TEndpoint endpoint = scope.ServiceProvider.GetService<TEndpoint>()
            .ThrowIfNull($"Endpoint \"{typeof(TEndpoint)}\" not found");
        endpoint.Configuration = configuration;
        endpoint.Configure(builder);

        return builder;
    }
}
