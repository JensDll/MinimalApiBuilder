using Fixture.TestApi.Features.Validation.Async;
using Fixture.TestApi.Features.Validation.Combination;
using Fixture.TestApi.Features.Validation.Sync;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MinimalApiBuilder;

namespace Fixture.TestApi.Features.Validation;

public static class Extensions
{
    public static void MapValidationFeatures(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder validation = endpoints.MapGroup("/validation").WithTags("Validation");
        validation.MapPost<SyncSingleEndpoint>("/sync/single");
        validation.MapPatch<SyncMultipleEndpoint>("/sync/multiple");
        validation.MapPost<AsyncSingleEndpoint>("/async/single");
        validation.MapPatch<AsyncMultipleEndpoint>("/async/multiple");
        validation.MapPut<CombinedEndpoint>("/combination");
    }
}
