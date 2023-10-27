using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using MinimalApiBuilder;
using Serilog;

namespace Fixture.TestApi.Features.Validation.Async;

public partial class AsyncSingleValidationEndpoint : MinimalApiBuilderEndpoint
{
    private readonly ILogger _logger;

    public AsyncSingleValidationEndpoint(ILogger logger)
    {
        _logger = logger;
    }

    private static async Task<IResult> HandleAsync(
        AsyncSingleValidationEndpoint endpoint,
        AsyncValidationRequest request,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        await Task.Delay(0, cancellationToken);
        MultipartReader? reader = MultipartReader.Create(context);
        return reader is not null ? Results.BadRequest() : Results.Ok();
    }

    public static void Configure(RouteHandlerBuilder builder)
    {
        builder.AddEndpointFilter(static (invocationContext, next) =>
        {
            var endpoint = invocationContext.GetArgument<AsyncSingleValidationEndpoint>(0);
            endpoint._logger.Information("Executing handler for {Endpoint}",
                nameof(AsyncSingleValidationEndpoint));
            return next(invocationContext);
        });
    }
}

public partial class AsyncMultipleValidationEndpoint : MinimalApiBuilderEndpoint
{
    private readonly ILogger _logger;

    public AsyncMultipleValidationEndpoint(ILogger logger)
    {
        _logger = logger;
    }

    private static Task<IResult> Handle(
        AsyncMultipleValidationEndpoint endpoint,
        AsyncValidationRequest request,
        [AsParameters] AsyncValidationParameters parameters,
        CancellationToken cancellationToken)
    {
        endpoint._logger.Information("Executing handler for {Endpoint}",
            nameof(AsyncMultipleValidationEndpoint));
        return Task.FromResult(Results.Ok());
    }
}
