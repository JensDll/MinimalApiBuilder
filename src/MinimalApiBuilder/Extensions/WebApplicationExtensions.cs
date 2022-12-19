using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace MinimalApiBuilder;

public static class WebApplicationExtensions
{
    public static RouteHandlerBuilder MapGet<TEndpoint>(this IEndpointRouteBuilder app, string pattern)
        where TEndpoint : EndpointBase
    {
        return app.Configure<TEndpoint>()
            .MapGet(pattern, EndpointHandlers.CreateHandler<TEndpoint>());
    }

    public static RouteHandlerBuilder MapPost<TEndpoint>(this IEndpointRouteBuilder app, string pattern)
        where TEndpoint : EndpointBase
    {
        return app.Configure<TEndpoint>()
            .MapPost(pattern, EndpointHandlers.CreateHandler<TEndpoint>());
    }

    public static RouteHandlerBuilder MapPut<TEndpoint>(this IEndpointRouteBuilder app, string pattern)
        where TEndpoint : EndpointBase
    {
        return app.Configure<TEndpoint>()
            .MapPut(pattern, EndpointHandlers.CreateHandler<TEndpoint>());
    }

    public static RouteHandlerBuilder MapDelete<TEndpoint>(this IEndpointRouteBuilder app, string pattern)
        where TEndpoint : EndpointBase
    {
        return app.Configure<TEndpoint>()
            .MapDelete(pattern, EndpointHandlers.CreateHandler<TEndpoint>());
    }

    private static IEndpointRouteBuilder Configure<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : EndpointBase
    {
        using var scope = app.ServiceProvider.CreateScope();

        var endpoint = scope.ServiceProvider.GetService<TEndpoint>()!;
        endpoint.Configure();

        return app;
    }
}

internal static class EndpointHandlers
{
    private static readonly Handler[] Handlers;

    static EndpointHandlers()
    {
        Assembly funcAssembly = Assembly.GetAssembly(typeof(Func<>))!;

        if (funcAssembly is null)
        {
            throw new InvalidOperationException("Could not find required type \"Func\"");
        }

        bool IsNameOfRequestHandler(MethodInfo method) =>
            method.Name is nameof(RequestHandler) or nameof(RequestWithParametersHandler);

        Handlers = typeof(EndpointHandlers).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .Where(IsNameOfRequestHandler)
            .Select(methodInfo =>
            {
                string funcTypeName = $"System.Func`{methodInfo.GetParameters().Length + 1}";
                Type? funcType = funcAssembly.GetType(funcTypeName, true);

                if (funcType is null)
                {
                    throw new InvalidOperationException($"Could not find type \"{funcTypeName}\"");
                }

                return new Handler
                {
                    MethodInfo = methodInfo,
                    FuncType = funcType
                };
            }).ToArray();
    }

    public static Delegate CreateHandler<TEndpoint>()
        where TEndpoint : EndpointBase
    {
        Type tEndpoint = typeof(TEndpoint);
        Type? tEndpointParent = tEndpoint.BaseType;

        if (tEndpointParent is null)
        {
            throw new InvalidOperationException(
                $"Endpoint \"{tEndpoint.Name}\" does not inherit from valid endpoint base");
        }

        EndpointTypeAttribute? endpointTypeAttribute =
            (EndpointTypeAttribute?)Attribute.GetCustomAttribute(tEndpointParent, typeof(EndpointTypeAttribute));

        if (endpointTypeAttribute is null)
        {
            throw new InvalidOperationException(
                $"Endpoint \"{tEndpoint.Name}\" does not inherit from valid endpoint base");
        }

        Type[] endpointGenericTypes = tEndpointParent.GenericTypeArguments;

        var (handlerFuncType, handlerMethodInfo) = endpointTypeAttribute.Type switch
        {
            EndpointType.Empty => (Handlers[0].FuncType, Handlers[0].MethodInfo.MakeGenericMethod(tEndpoint)),
            EndpointType.Request => (Handlers[1].FuncType,
                Handlers[1].MethodInfo.MakeGenericMethod(tEndpoint, endpointGenericTypes[0])),
            EndpointType.Parameters => (Handlers[2].FuncType,
                Handlers[2].MethodInfo.MakeGenericMethod(tEndpoint, endpointGenericTypes[0])),
            EndpointType.RequestWithParameters => (Handlers[3].FuncType,
                Handlers[3].MethodInfo.MakeGenericMethod(tEndpoint, endpointGenericTypes[0], endpointGenericTypes[1])),
            _ => throw new InvalidOperationException(
                $"Endpoint \"{tEndpoint.Name}\" does not inherit from valid endpoint base")
        };

        Type[] funcArguments = handlerMethodInfo.GetParameters()
            .Select(parameterInfo => parameterInfo.ParameterType)
            .Append(handlerMethodInfo.ReturnType).ToArray();

        return Delegate.CreateDelegate(handlerFuncType.MakeGenericType(funcArguments), handlerMethodInfo);
    }

    private static Task<IResult> RequestHandler<TEndpoint>(HttpContext context, TEndpoint endpoint,
        CancellationToken cancellationToken)
        where TEndpoint : Endpoint
    {
        return endpoint.ExecuteAsync(context, cancellationToken);
    }

    private static Task<IResult> RequestHandler<TEndpoint, TRequest>(TRequest request, HttpContext context,
        TEndpoint endpoint, CancellationToken cancellationToken)
        where TEndpoint : Endpoint<TRequest>
        where TRequest : notnull
    {
        return endpoint.ExecuteAsync(request, context, cancellationToken);
    }

    private static Task<IResult> RequestWithParametersHandler<TEndpoint, TParameters>(
        [AsParameters] TParameters parameters, HttpContext context, TEndpoint endpoint,
        CancellationToken cancellationToken)
        where TEndpoint : EndpointWithParameters<TParameters>
        where TParameters : notnull
    {
        return endpoint.ExecuteAsync(parameters, context, cancellationToken);
    }

    private static Task<IResult> RequestWithParametersHandler<TEndpoint, TRequest, TParameters>(TRequest request,
        [AsParameters] TParameters parameters, HttpContext context, TEndpoint endpoint,
        CancellationToken cancellationToken)
        where TEndpoint : EndpointWithParameters<TRequest, TParameters>
        where TRequest : notnull
        where TParameters : notnull
    {
        return endpoint.ExecuteAsync(request, parameters, context, cancellationToken);
    }

    private class Handler
    {
        public required MethodInfo MethodInfo { get; set; }
        public required Type FuncType { get; init; }
    }
}
