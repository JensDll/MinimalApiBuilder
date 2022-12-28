using MinimalApiBuilder;
using ILogger = Serilog.ILogger;

namespace Sample.WebApi.Features.Validation.Async;

public partial class ValidationAsync : MinimalApiBuilderEndpoint
{
    private readonly ILogger _logger;

    public ValidationAsync(ILogger logger)
    {
        _logger = logger;
    }

    private static Task<IResult> Handle(Request request, [AsParameters] Parameters parameters,
        ValidationAsync endpoint, CancellationToken cancellationToken)
    {
        endpoint._logger.Information("Handling async validation request");
        return Task.FromResult(Results.Ok());
    }

    public static void Configure(RouteHandlerBuilder builder) { }
}
