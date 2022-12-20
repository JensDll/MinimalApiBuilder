using ILogger = Serilog.ILogger;

namespace Web.Features.Customer.List;

public class CustomerList : MinimalApiBuilder.EndpointWithParameters<Parameters>
{
    private readonly ILogger _logger;

    public CustomerList(ILogger logger)
    {
        _logger = logger;
    }

    protected override Task<IResult> HandleAsync(Parameters parameters, CancellationToken cancellationToken)
    {
        _logger.Information("Customer list endpoint");
        return Task.FromResult(Results.Ok(parameters));
    }
}
