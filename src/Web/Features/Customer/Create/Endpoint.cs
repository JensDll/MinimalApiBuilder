namespace Web.Features.Customer.Create;

public class CustomerCreate : MinimalApiBuilder.Endpoint
{
    private readonly ILogger<CustomerCreate> _logger;

    public CustomerCreate(ILogger<CustomerCreate> logger)
    {
        _logger = logger;
    }

    protected override Task<IResult> HandleAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Customer Create");
        return Task.FromResult(Results.Ok());
    }
}
