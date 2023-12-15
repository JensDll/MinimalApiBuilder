using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MinimalApiBuilder.Generator;

namespace Fixture.TestApi.Features.Validation;

internal partial class CombinedValidationEndpoint : MinimalApiBuilderEndpoint
{
    public static IResult Handle(
        [FromServices] ILogger<CombinedValidationEndpoint> logger,
        [AsParameters] SyncValidationParameters parameters,
        AsyncValidationRequest request)
    {
        logger.SyncValidationParameters(parameters);
        logger.AsyncValidationRequest(request);
        return TypedResults.Ok();
    }
}
