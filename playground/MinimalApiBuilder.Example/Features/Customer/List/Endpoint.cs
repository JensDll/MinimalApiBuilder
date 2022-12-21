using MinimalApiBuilder;
using ILogger = Serilog.ILogger;

namespace Web.Features.Customer.List;

public class CustomerList : Endpoint<CustomerList>
{
    private readonly ILogger _logger;

    public CustomerList(ILogger logger)
    {
        _logger = logger;
    }

    public static Task<IResult> HandleAsync([AsParameters] Parameters parameters, CustomerList endpoint,
        CancellationToken cancellationToken)
    {
        endpoint._logger.Information("Customer list endpoint");
        return Task.FromResult(Results.Ok(parameters));
    }
}
