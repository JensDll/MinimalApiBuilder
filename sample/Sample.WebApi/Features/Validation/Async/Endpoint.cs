using MinimalApiBuilder;
using ILogger = Serilog.ILogger;

namespace Sample.WebApi.Features.Validation.Async;

public class ValidationAsync : Endpoint<ValidationAsync>, ITest
{
    private readonly ILogger _logger;

    public ValidationAsync(ILogger logger)
    {
        _logger = logger;
    }

    public static Task<IResult> HandleAsync(Request request, [AsParameters] Parameters parameters,
        ValidationAsync endpoint, CancellationToken cancellationToken)
    {
        return Task.FromResult(Results.Ok());
    }
}

public interface ITest { }
