using Fixture.TestApi.Features.Validation.Async;
using Fixture.TestApi.Features.Validation.Sync;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using MinimalApiBuilder;

namespace Fixture.TestApi.Features.Validation.Combination;

public partial class CombinedValidationEndpoint : MinimalApiBuilderEndpoint
{
    private static IResult Handle(AsyncValidationRequest request,
        [AsParameters] SyncValidationParameters parameters,
        CombinedValidationEndpoint endpoint)
    {
        return Results.Ok();
    }

    public static void Configure(RouteHandlerBuilder builder)
    { }
}
