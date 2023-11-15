using Microsoft.AspNetCore.Http;
using MinimalApiBuilder;

namespace Fixture.TestApi.Features.Validation;

internal partial class CombinedValidationEndpoint : MinimalApiBuilderEndpoint
{
    public static IResult Handle(
        [AsParameters] SyncValidationParameters parameters,
        AsyncValidationRequest request,
        Serilog.ILogger logger)
    {
        logger.Information("Parameters: {Parameters}", parameters);
        logger.Information("Request: {Request}", request);
        return TypedResults.Ok();
    }
}
