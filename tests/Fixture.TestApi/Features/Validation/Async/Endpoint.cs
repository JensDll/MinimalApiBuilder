using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MinimalApiBuilder;
using Serilog;

namespace Fixture.TestApi.Features.Validation.Async;

public partial class AsyncSingleValidationEndpoint : MinimalApiBuilderEndpoint
{
    private static async Task<Results<Ok<ICollection<string>>, BadRequest>> HandleAsync(
        [FromServices] AsyncSingleValidationEndpoint endpoint,
        AsyncValidationRequest request,
        HttpContext context,
        [FromServices] ILogger logger,
        CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        MultipartReader _ = new(context, endpoint);
        return TypedResults.Ok(endpoint.ValidationErrors);
    }

    public static void Configure(RouteHandlerBuilder builder)
    {
        builder.AddEndpointFilter(static (invocationContext, next) =>
        {
            ILogger logger = invocationContext.GetArgument<ILogger>(3);
            logger.Information("Executing handler for {Endpoint}", nameof(AsyncSingleValidationEndpoint));
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
