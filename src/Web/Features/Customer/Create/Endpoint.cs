using ILogger = Serilog.ILogger;

namespace Web.Features.Customer.Create;

public class CustomerCreate : MinimalApiBuilder.Endpoint
{
    private readonly ILogger _logger;

    public CustomerCreate(ILogger logger)
    {
        _logger = logger;
    }

    protected override Task<IResult> HandleAsync(CancellationToken cancellationToken)
    {
        _logger.Information("Customer create endpoint");
        return Task.FromResult(Results.Ok());
    }
}
