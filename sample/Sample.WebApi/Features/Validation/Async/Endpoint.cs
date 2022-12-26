using MinimalApiBuilder;
using ILogger = Serilog.ILogger;

namespace Sample.WebApi.Features.Validation.Async;

public partial class ValidationAsync : Endpoint<ValidationAsync>
{
    private readonly ILogger _logger;

    public ValidationAsync(ILogger logger)
    {
        _logger = logger;
    }

    private static Task<IResult> HandleAsync(Request request, [AsParameters] Parameters parameters,
        ValidationAsync endpoint, CancellationToken cancellationToken)
    {
        return Task.FromResult(Results.Ok());
    }
}
