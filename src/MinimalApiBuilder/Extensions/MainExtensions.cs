using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace MinimalApiBuilder;

public static class MainExtensions
{
    public static RouteHandlerBuilder MapGet<TEndpoint>(this IEndpointRouteBuilder app, string pattern)
        where TEndpoint : EndpointBase, IEndpoint
    {
        return app
            .MapGet(pattern, EndpointHandler.Create<TEndpoint>())
            .Configure<TEndpoint>(app.ServiceProvider, HttpVerbs.Get);
    }

    public static RouteHandlerBuilder MapPost<TEndpoint>(this IEndpointRouteBuilder app, string pattern)
        where TEndpoint : EndpointBase, IEndpoint
    {
        return app
            .MapPost(pattern, EndpointHandler.Create<TEndpoint>())
            .Configure<TEndpoint>(app.ServiceProvider, HttpVerbs.Post);
    }

    public static RouteHandlerBuilder MapPut<TEndpoint>(this IEndpointRouteBuilder app, string pattern)
        where TEndpoint : EndpointBase, IEndpoint
    {
        return app
            .MapPut(pattern, EndpointHandler.Create<TEndpoint>())
            .Configure<TEndpoint>(app.ServiceProvider, HttpVerbs.Put);
    }

    public static RouteHandlerBuilder MapPatch<TEndpoint>(this IEndpointRouteBuilder app, string pattern)
        where TEndpoint : EndpointBase, IEndpoint
    {
        return app
            .MapPatch(pattern, EndpointHandler.Create<TEndpoint>())
            .Configure<TEndpoint>(app.ServiceProvider, HttpVerbs.Patch);
    }

    public static RouteHandlerBuilder MapDelete<TEndpoint>(this IEndpointRouteBuilder app, string pattern)
        where TEndpoint : EndpointBase, IEndpoint
    {
        return app
            .MapDelete(pattern, EndpointHandler.Create<TEndpoint>())
            .Configure<TEndpoint>(app.ServiceProvider, HttpVerbs.Delete);
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

        IEnumerable<Type> funcArgumentsEnumerable = TEndpoint.ArgumentPositions.Count == 0
            ? parameters.Select((parameterInfo, index) =>
            {
                Type parameterType = parameterInfo.ParameterType;
                TEndpoint.ArgumentPositions.Add(parameterType, index);
                return parameterType;
            })
            : parameters.Select(parameterInfo => parameterInfo.ParameterType);

        Type[] funcArguments = funcArgumentsEnumerable.Append(handler.ReturnType).ToArray();

        return Delegate.CreateDelegate(funcType.MakeGenericType(funcArguments), handler);
    }
}
