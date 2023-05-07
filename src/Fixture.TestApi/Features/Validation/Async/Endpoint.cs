﻿using MinimalApiBuilder;
using ILogger = Serilog.ILogger;

namespace Fixture.TestApi.Features.Validation.Async;

public partial class AsyncSingleEndpoint : MinimalApiBuilderEndpoint
{
    private readonly ILogger _logger;

    public AsyncSingleEndpoint(ILogger logger)
    {
        _logger = logger;
    }

    private static async Task<IResult> HandleAsync(
        AsyncSingleEndpoint endpoint,
        Request request,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        await Task.Delay(0, cancellationToken);
        MultipartReader? reader = MultipartReader.Create(context);
        return reader is not null ? Results.BadRequest() : Results.Ok();
    }

    public static void Configure(RouteHandlerBuilder builder)
    {
        builder.AddEndpointFilter(static (invocationContext, next) =>
        {
            var endpoint = invocationContext.GetArgument<AsyncSingleEndpoint>(0);
            endpoint._logger.Information("Executing handler for {Endpoint}", nameof(Endpoint));
            return next(invocationContext);
        });
    }
}

public partial class AsyncMultipleEndpoint : MinimalApiBuilderEndpoint
{
    private readonly ILogger _logger;

    public AsyncMultipleEndpoint(ILogger logger)
    {
        _logger = logger;
    }

    private static Task<IResult> HandleAsync(
        AsyncMultipleEndpoint endpoint,
        Request request,
        [AsParameters] Parameters parameters,
        CancellationToken cancellationToken)
    {
        endpoint._logger.Information("Executing handler for {Endpoint}",
            nameof(AsyncMultipleEndpoint));
        return Task.FromResult(Results.Ok());
    }
}
