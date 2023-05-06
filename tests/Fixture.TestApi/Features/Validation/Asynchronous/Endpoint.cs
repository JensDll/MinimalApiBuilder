using MinimalApiBuilder;
using ILogger = Serilog.ILogger;

namespace Fixture.TestApi.Features.Validation.Asynchronous;

public partial class SingleEndpoint : MinimalApiBuilderEndpoint
{
    private readonly ILogger _logger;

    public SingleEndpoint(ILogger logger)
    {
        _logger = logger;
    }

    private static async Task<IResult> HandleAsync(
        SingleEndpoint endpoint,
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
            var endpoint = invocationContext.GetArgument<SingleEndpoint>(0);
            endpoint._logger.Information("Executing handler for {Endpoint}", nameof(Endpoint));
            return next(invocationContext);
        });
    }
}

public partial class AsynchronousMultipleValidationEndpoint : MinimalApiBuilderEndpoint
{
    private readonly ILogger _logger;

    public AsynchronousMultipleValidationEndpoint(ILogger logger)
    {
        _logger = logger;
    }

    private static Task<IResult> HandleAsync(
        AsynchronousMultipleValidationEndpoint endpoint,
        Request request,
        [AsParameters] Parameters parameters,
        CancellationToken cancellationToken)
    {
        endpoint._logger.Information("Executing handler for {Endpoint}",
            nameof(AsynchronousMultipleValidationEndpoint));
        return Task.FromResult(Results.Ok());
    }

    public static void Configure(RouteHandlerBuilder builder) { }
}
