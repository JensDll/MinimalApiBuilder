using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace MinimalApiBuilder;

public static class WebApplicationExtensions
{
    public static RouteHandlerBuilder MapGet<TEndpoint>(this IEndpointRouteBuilder app, string pattern)
        where TEndpoint : EndpointBase =>
        app.Configure<TEndpoint>().MapGet(pattern, EndpointHandlers.CreateHandler<TEndpoint>());

    public static RouteHandlerBuilder MapPost<TEndpoint>(this IEndpointRouteBuilder app, string pattern)
        where TEndpoint : EndpointBase =>
        app.Configure<TEndpoint>().MapPost(pattern, EndpointHandlers.CreateHandler<TEndpoint>());

    public static RouteHandlerBuilder MapPut<TEndpoint>(this IEndpointRouteBuilder app, string pattern)
        where TEndpoint : EndpointBase =>
        app.Configure<TEndpoint>().MapPut(pattern, EndpointHandlers.CreateHandler<TEndpoint>());

    public static RouteHandlerBuilder MapDelete<TEndpoint>(this IEndpointRouteBuilder app, string pattern)
        where TEndpoint : EndpointBase =>
        app.Configure<TEndpoint>().MapDelete(pattern, EndpointHandlers.CreateHandler<TEndpoint>());

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
        bool IsRequestHandler(MethodInfo method) =>
            method.Name is nameof(RequestHandler) or nameof(RequestWithParametersHandler);

        EndpointType HandlerType(Handler handler) => handler.Type;

        Handlers = typeof(EndpointHandlers).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .Where(IsRequestHandler)
            .Select(Handler.Construct)
            .OrderBy(HandlerType)
            .ToArray();
    }

    public static Delegate CreateHandler<TEndpoint>()
        where TEndpoint : EndpointBase
    {
        Type tEndpoint = typeof(TEndpoint);
        Type tEndpointParent =
            tEndpoint.BaseType.ThrowIfNull($"Endpoint \"{tEndpoint}\" does not have valid base type");

        EndpointTypeAttribute endpointTypeAttribute = tEndpointParent.GetCustomAttribute<EndpointTypeAttribute>();

        Type[] genericArguments = new Type[tEndpointParent.GenericTypeArguments.Length + 1];
        genericArguments[0] = tEndpoint;
        tEndpointParent.GenericTypeArguments.CopyTo(genericArguments, 1);

        Debug.Assert(Handlers[(int)endpointTypeAttribute.Type].Type == endpointTypeAttribute.Type);

        return Handlers[(int)endpointTypeAttribute.Type].AsDelegate(genericArguments);
    }

    private static Task<IResult> RequestHandler<TEndpoint>(
        HttpContext context,
        TEndpoint endpoint,
        CancellationToken cancellationToken)
        where TEndpoint : Endpoint => endpoint.ExecuteAsync(context, cancellationToken);

    private static Task<IResult> RequestHandler<TEndpoint, TRequest>(
        TRequest request,
        HttpContext context,
        TEndpoint endpoint,
        CancellationToken cancellationToken)
        where TEndpoint : Endpoint<TRequest>
        where TRequest : notnull => endpoint.ExecuteAsync(request, context, cancellationToken);

    private static Task<IResult> RequestWithParametersHandler<TEndpoint, TParameters>(
        [AsParameters] TParameters parameters,
        HttpContext context,
        TEndpoint endpoint,
        CancellationToken cancellationToken)
        where TEndpoint : EndpointWithParameters<TParameters>
        where TParameters : notnull => endpoint.ExecuteAsync(parameters, context, cancellationToken);

    private static Task<IResult> RequestWithParametersHandler<TEndpoint, TRequest, TParameters>(
        TRequest request,
        [AsParameters] TParameters parameters,
        HttpContext context,
        TEndpoint endpoint,
        CancellationToken cancellationToken)
        where TEndpoint : EndpointWithParameters<TRequest, TParameters>
        where TRequest : notnull
        where TParameters : notnull => endpoint.ExecuteAsync(request, parameters, context, cancellationToken);

    private class Handler
    {
        private static readonly Assembly FuncAssembly = Assembly.GetAssembly(typeof(Func<>))
            .ThrowIfNull($"Could not find required delegate type \"{typeof(Func<>)}\"");

        private readonly MethodInfo _methodInfo;
        private readonly Type _funcType;

        private Handler(MethodInfo methodInfo)
        {
            string funcTypeName = $"System.Func`{methodInfo.GetParameters().Length + 1}";
            Type funcType = FuncAssembly.GetType(funcTypeName).ThrowIfNull($"Could not find type \"{funcTypeName}\"");

            Type endpointType = methodInfo.GetGenericArguments()[0].GetGenericParameterConstraints()[0];

            Debug.Assert(endpointType.IsSubclassOf(typeof(EndpointBase)));

            EndpointTypeAttribute endpointTypeAttribute = endpointType.GetCustomAttribute<EndpointTypeAttribute>();

            _methodInfo = methodInfo;
            _funcType = funcType;
            Type = endpointTypeAttribute.Type;
        }

        private Handler(MethodInfo methodInfo, Type funcType, EndpointType type)
        {
            _methodInfo = methodInfo;
            _funcType = funcType;
            Type = type;
        }

        public EndpointType Type { get; }

        public static Handler Construct(MethodInfo methodInfo) => new(methodInfo);

        public void Deconstruct(out MethodInfo methodInfo, out Type funcType)
        {
            methodInfo = _methodInfo;
            funcType = _funcType;
        }

        private Handler MakeGeneric(params Type[] genericArguments)
        {
            MethodInfo methodInfo = _methodInfo.MakeGenericMethod(genericArguments);

            Type[] funcArguments = methodInfo.GetParameters()
                .Select(parameterInfo => parameterInfo.ParameterType)
                .Append(methodInfo.ReturnType)
                .ToArray();

            Type funcType = _funcType.MakeGenericType(funcArguments);

            return new Handler(methodInfo, funcType, Type);
        }

        public Delegate AsDelegate(params Type[] genericArguments)
        {
            (MethodInfo handlerMethodInfo, Type handlerFuncType) = MakeGeneric(genericArguments);
            return Delegate.CreateDelegate(handlerFuncType, handlerMethodInfo);
        }
    }
}
