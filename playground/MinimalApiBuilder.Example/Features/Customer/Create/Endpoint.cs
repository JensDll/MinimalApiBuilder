using MinimalApiBuilder;
using ILogger = Serilog.ILogger;

namespace Web.Features.Customer.Create;

public class CustomerCreate : Endpoint<CustomerCreate>
{
    private readonly ILogger _logger;

    public CustomerCreate(ILogger logger)
    {
        _logger = logger;
    }

    public static Task<IResult> HandleAsync(CustomerCreate endpoint, CancellationToken cancellationToken)
    {
        endpoint._logger.Information("Customer create endpoint");
        return Task.FromResult(Results.Ok());
    }
}
