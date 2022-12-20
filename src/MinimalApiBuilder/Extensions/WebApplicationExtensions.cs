using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace MinimalApiBuilder;

public static class WebApplicationExtensions
{
    public static RouteHandlerBuilder MapGet<TEndpoint>(this IEndpointRouteBuilder app, string pattern)
        where TEndpoint : EndpointBase, IEndpoint => app
        .MapGet(pattern, EndpointHandler.Create<TEndpoint>())
        .Configure<TEndpoint>(app.ServiceProvider);

    public static RouteHandlerBuilder MapPost<TEndpoint>(this IEndpointRouteBuilder app, string pattern)
        where TEndpoint : EndpointBase, IEndpoint => app
        .MapPost(pattern, EndpointHandler.Create<TEndpoint>())
        .Configure<TEndpoint>(app.ServiceProvider);

    public static RouteHandlerBuilder MapPut<TEndpoint>(this IEndpointRouteBuilder app, string pattern)
        where TEndpoint : EndpointBase, IEndpoint => app
        .MapPut(pattern, EndpointHandler.Create<TEndpoint>())
        .Configure<TEndpoint>(app.ServiceProvider);

    public static RouteHandlerBuilder MapPatch<TEndpoint>(this IEndpointRouteBuilder app, string pattern)
        where TEndpoint : EndpointBase, IEndpoint => app
        .MapPatch(pattern, EndpointHandler.Create<TEndpoint>())
        .Configure<TEndpoint>(app.ServiceProvider);

    public static RouteHandlerBuilder MapDelete<TEndpoint>(this IEndpointRouteBuilder app, string pattern)
        where TEndpoint : EndpointBase, IEndpoint => app
        .MapDelete(pattern, EndpointHandler.Create<TEndpoint>())
        .Configure<TEndpoint>(app.ServiceProvider);

    private static RouteHandlerBuilder Configure<TEndpoint>(
        this RouteHandlerBuilder builder,
        IServiceProvider serviceProvider)
        where TEndpoint : EndpointBase, IEndpoint
    {
        EndpointConfiguration configuration = new();

        int endpointArgumentIndex = TEndpoint.ArgumentPositions[typeof(TEndpoint)];

        builder.AddEndpointFilter((context, next) =>
        {
            TEndpoint endpoint = context.GetArgument<TEndpoint>(endpointArgumentIndex);
            endpoint.Configuration = configuration;
            return next(context);
        });

        using var scope = serviceProvider.CreateScope();

        TEndpoint endpoint = scope.ServiceProvider.GetService<TEndpoint>()!;
        endpoint.Configuration = configuration;
        endpoint.Configure(builder);

        return builder;
    }
}

internal static class EndpointHandler
{
    private static readonly Assembly FuncAssembly = Assembly.GetAssembly(typeof(Func<>))
        .ThrowIfNull($"Could not find required delegate type \"{typeof(Func<>)}\"");


    public static Delegate Create<TEndpoint>()
        where TEndpoint : IEndpoint
    {
        Type tEndpoint = typeof(TEndpoint);

        MethodInfo? handle = tEndpoint.GetMethod("Handle", BindingFlags.Static | BindingFlags.Public);
        MethodInfo? handleAsync = tEndpoint.GetMethod("HandleAsync", BindingFlags.Static | BindingFlags.Public);
        MethodInfo handler = (handle ?? handleAsync).ThrowIfNull("Could not find a suitable handler method");

        ParameterInfo[] parameters = handler.GetParameters();

        string funcTypeName = $"System.Func`{parameters.Length + 1}";
        Type funcType = FuncAssembly.GetType(funcTypeName).ThrowIfNull($"Could not find type \"{funcTypeName}\"");

        Type[] funcArguments = parameters
            .Select((parameterInfo, i) =>
            {
                Type parameterType = parameterInfo.ParameterType;
                TEndpoint.ArgumentPositions.Add(parameterType, i);
                return parameterType;
            })
            .Append(handler.ReturnType)
            .ToArray();

        return Delegate.CreateDelegate(funcType.MakeGenericType(funcArguments), handler);
    }
}
