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
        where TEndpoint : EndpointBase
    {
        return app.MapGet(pattern, EndpointHandlers.CreateHandler<TEndpoint>(out int endpointArgumentIndex))
            .Configure<TEndpoint>(app.ServiceProvider, endpointArgumentIndex);
    }


    public static RouteHandlerBuilder MapPost<TEndpoint>(this IEndpointRouteBuilder app, string pattern)
        where TEndpoint : EndpointBase
    {
        return app.MapPost(pattern, EndpointHandlers.CreateHandler<TEndpoint>(out int endpointArgumentIndex))
            .Configure<TEndpoint>(app.ServiceProvider, endpointArgumentIndex);
    }


    public static RouteHandlerBuilder MapPut<TEndpoint>(this IEndpointRouteBuilder app, string pattern)
        where TEndpoint : EndpointBase
    {
        return app.MapPut(pattern, EndpointHandlers.CreateHandler<TEndpoint>(out int endpointArgumentIndex))
            .Configure<TEndpoint>(app.ServiceProvider, endpointArgumentIndex);
    }

    public static RouteHandlerBuilder MapPatch<TEndpoint>(this IEndpointRouteBuilder app, string pattern)
        where TEndpoint : EndpointBase
    {
        return app.MapPatch(pattern, EndpointHandlers.CreateHandler<TEndpoint>(out int endpointArgumentIndex))
            .Configure<TEndpoint>(app.ServiceProvider, endpointArgumentIndex);
    }

    public static RouteHandlerBuilder MapDelete<TEndpoint>(this IEndpointRouteBuilder app, string pattern)
        where TEndpoint : EndpointBase
    {
        return app.MapDelete(pattern, EndpointHandlers.CreateHandler<TEndpoint>(out int endpointArgumentIndex))
            .Configure<TEndpoint>(app.ServiceProvider, endpointArgumentIndex);
    }

    private static RouteHandlerBuilder Configure<TEndpoint>(this RouteHandlerBuilder builder,
        IServiceProvider serviceProvider, int endpointArgumentIndex)
        where TEndpoint : EndpointBase
    {
        EndpointConfiguration configuration = new();

        builder.AddEndpointFilter((context, next) =>
        {
            TEndpoint endpoint = context.GetArgument<TEndpoint>(endpointArgumentIndex);
            endpoint.Assign(configuration, context.HttpContext);
            return next(context);
        });

        using var scope = serviceProvider.CreateScope();

        TEndpoint endpoint = scope.ServiceProvider.GetService<TEndpoint>()!;
        endpoint.Assign(configuration, new DefaultHttpContext());
        endpoint.Configure(builder);

        return builder;
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

    public static Delegate CreateHandler<TEndpoint>(out int endpointArgumentIndex)
        where TEndpoint : EndpointBase
    {
        Type tEndpoint = typeof(TEndpoint);
        Type tEndpointParent =
            tEndpoint.BaseType.ThrowIfNull($"Endpoint \"{tEndpoint}\" does not have valid base type");

        EndpointTypeAttribute endpointTypeAttribute = tEndpointParent.GetCustomAttribute<EndpointTypeAttribute>();

        Type[] genericArguments = new Type[tEndpointParent.GenericTypeArguments.Length + 1];
        genericArguments[0] = tEndpoint;
        tEndpointParent.GenericTypeArguments.CopyTo(genericArguments, 1);

        Handler handler = Handlers[(int)endpointTypeAttribute.Type];

        Debug.Assert(handler.Type == endpointTypeAttribute.Type);

        endpointArgumentIndex = handler.ParameterCount - 2;

        return handler.AsDelegate(genericArguments);
    }

    private static Task<IResult> RequestHandler<TEndpoint>(
        TEndpoint endpoint,
        CancellationToken cancellationToken)
        where TEndpoint : Endpoint => endpoint.ExecuteAsync(cancellationToken);

    private static Task<IResult> RequestHandler<TEndpoint, TRequest>(
        TRequest request,
        TEndpoint endpoint,
        CancellationToken cancellationToken)
        where TEndpoint : Endpoint<TRequest>
        where TRequest : notnull => endpoint.ExecuteAsync(request, cancellationToken);

    private static Task<IResult> RequestWithParametersHandler<TEndpoint, TParameters>(
        [AsParameters] TParameters parameters,
        TEndpoint endpoint,
        CancellationToken cancellationToken)
        where TEndpoint : EndpointWithParameters<TParameters>
        where TParameters : notnull => endpoint.ExecuteAsync(parameters, cancellationToken);

    private static Task<IResult> RequestWithParametersHandler<TEndpoint, TRequest, TParameters>(
        TRequest request,
        [AsParameters] TParameters parameters,
        TEndpoint endpoint,
        CancellationToken cancellationToken)
        where TEndpoint : EndpointWithParameters<TRequest, TParameters>
        where TRequest : notnull
        where TParameters : notnull => endpoint.ExecuteAsync(request, parameters, cancellationToken);

    private class Handler
    {
        private static readonly Assembly FuncAssembly = Assembly.GetAssembly(typeof(Func<>))
            .ThrowIfNull($"Could not find required delegate type \"{typeof(Func<>)}\"");

        private readonly MethodInfo _methodInfo;
        private readonly Type _funcType;

        private Handler(MethodInfo methodInfo)
        {
            ParameterCount = methodInfo.GetParameters().Length;

            string funcTypeName = $"System.Func`{ParameterCount + 1}";
            Type funcType = FuncAssembly.GetType(funcTypeName).ThrowIfNull($"Could not find type \"{funcTypeName}\"");

            Type endpointType = methodInfo.GetGenericArguments()[0].GetGenericParameterConstraints()[0];

            Debug.Assert(endpointType.IsSubclassOf(typeof(EndpointBase)));

            EndpointTypeAttribute endpointTypeAttribute = endpointType.GetCustomAttribute<EndpointTypeAttribute>();

            _methodInfo = methodInfo;
            _funcType = funcType;
            Type = endpointTypeAttribute.Type;
        }

        private Handler(MethodInfo methodInfo, Type funcType, EndpointType type, int parameterCount)
        {
            _methodInfo = methodInfo;
            _funcType = funcType;
            Type = type;
            ParameterCount = parameterCount;
        }

        public EndpointType Type { get; }

        public int ParameterCount { get; }

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

            return new Handler(methodInfo, funcType, Type, ParameterCount);
        }

        public Delegate AsDelegate(params Type[] genericArguments)
        {
            var (handlerMethodInfo, handlerFuncType) = MakeGeneric(genericArguments);
            return Delegate.CreateDelegate(handlerFuncType, handlerMethodInfo);
        }
    }
}
