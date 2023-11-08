using Microsoft.AspNetCore.Http;
using MinimalApiBuilder;

namespace Fixture.TestApi.Features.Validation.Sync;

internal partial class SyncSingleValidationEndpoint : MinimalApiBuilderEndpoint
{
    private static IResult Handle(
        SyncValidationRequest request,
        SyncSingleValidationEndpoint endpoint)
    {
        return Results.Ok();
    }
}

internal partial class SyncMultipleValidationEndpoint : MinimalApiBuilderEndpoint
{
    private static IResult Handle(
        SyncValidationRequest request,
        [AsParameters] SyncValidationParameters parameters,
        SyncMultipleValidationEndpoint endpoint)
    {
        return Results.Ok();
    }
}
