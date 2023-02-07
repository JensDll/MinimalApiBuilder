using MinimalApiBuilder;
using ILogger = Serilog.ILogger;

namespace Fixture.TestApi.Features.Validation.Asynchronous;

public partial class EndpointSingle : MinimalApiBuilderEndpoint
{
    private readonly ILogger _logger;

    public EndpointSingle(ILogger logger)
    {
        _logger = logger;
    }

    private static async Task<IResult> HandleAsync(
        EndpointSingle endpoint,
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
            EndpointSingle endpoint = invocationContext.GetArgument<EndpointSingle>(0);
            endpoint._logger.Information("Executing handler for {Endpoint}", nameof(Endpoint));
            return next(invocationContext);
        });
    }
}

public partial class EndpointMultiple : MinimalApiBuilderEndpoint
{
    private readonly ILogger _logger;

    public EndpointMultiple(ILogger logger)
    {
        _logger = logger;
    }

    private static Task<IResult> HandleAsync(
        EndpointMultiple endpoint,
        Request request,
        [AsParameters] Parameters parameters,
        CancellationToken cancellationToken)
    {
        endpoint._logger.Information("Executing handler for {Endpoint}",
            nameof(EndpointMultiple));
        return Task.FromResult(Results.Ok());
    }

    public static void Configure(RouteHandlerBuilder builder) { }
}
