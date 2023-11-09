using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinimalApiBuilder;

namespace Fixture.TestApi.Features.Validation;

internal partial class CombinedValidationEndpoint : MinimalApiBuilderEndpoint
{
    private static IResult Handle(
        [AsParameters] SyncValidationParameters parameters,
        AsyncValidationRequest request,
        [FromServices] CombinedValidationEndpoint endpoint)
    {
        return TypedResults.Ok();
    }
}
