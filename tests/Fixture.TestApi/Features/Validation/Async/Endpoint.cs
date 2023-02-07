using MinimalApiBuilder;
using ILogger = Serilog.ILogger;

namespace Sample.WebApi.Features.Validation.Async;

public partial class AsyncValidationSingleEndpoint : MinimalApiBuilderEndpoint
{
    private readonly ILogger _logger;

    public AsyncValidationSingleEndpoint(ILogger logger)
    {
        _logger = logger;
    }

    private static async Task<IResult> HandleAsync(
        AsyncValidationSingleEndpoint endpoint,
        AsyncValidationRequest request,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        await Task.Delay(0, cancellationToken);
        MultipartReader? reader = MultipartReader.Create(context);

        if (reader is not null)
        {
            return Results.BadRequest();
        }

        return Results.Ok();
    }

    public static void Configure(RouteHandlerBuilder builder)
    {
        builder.AddEndpointFilter(static (invocationContext, next) =>
        {
            AsyncValidationSingleEndpoint endpoint = invocationContext.GetArgument<AsyncValidationSingleEndpoint>(0);
            endpoint._logger.Information("Executing handler for {Endpoint}",
                nameof(AsyncValidationSingleEndpoint));
            return next(invocationContext);
        });
    }
}

public partial class AsyncValidationMultipleEndpoint : MinimalApiBuilderEndpoint
{
    private readonly ILogger _logger;

    public AsyncValidationMultipleEndpoint(ILogger logger)
    {
        _logger = logger;
    }

    private static Task<IResult> HandleAsync(
        AsyncValidationMultipleEndpoint endpoint,
        AsyncValidationRequest request,
        [AsParameters] AsyncValidationParameters parameters,
        CancellationToken cancellationToken)
    {
        endpoint._logger.Information("Executing handler for {Endpoint}",
            nameof(AsyncValidationMultipleEndpoint));
        return Task.FromResult(Results.Ok());
    }

    public static void Configure(RouteHandlerBuilder builder) { }
}
