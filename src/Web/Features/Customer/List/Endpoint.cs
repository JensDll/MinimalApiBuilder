namespace Web.Features.Customer.List;

public class CustomerList : MinimalApiBuilder.EndpointWithParameters<Parameters>
{
    private readonly ILogger<CustomerList> _logger;

    public CustomerList(ILogger<CustomerList> logger)
    {
        _logger = logger;
    }

    protected override Task<IResult> HandleAsync(Parameters parameters, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Customer List");
        return Task.FromResult(Results.Ok(parameters));
    }
}
