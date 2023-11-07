using Fixture.TestApi.Features.Validation.Async;
using Fixture.TestApi.Features.Validation.Combination;
using Fixture.TestApi.Features.Validation.Sync;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MinimalApiBuilder;

namespace Fixture.TestApi.Features.Validation;

internal static class ValidationFeatureExtensions
{
    public static void MapValidationFeatures(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder validation = endpoints.MapGroup("/validation").WithTags("Validation");
        validation.MapPost<SyncSingleValidationEndpoint>("/sync/single");
        validation.MapPatch<SyncMultipleValidationEndpoint>("/sync/multiple");
        validation.MapPost<AsyncSingleValidationEndpoint>("/async/single");
        validation.MapPatch<AsyncMultipleValidationEndpoint>("/async/multiple");
        validation.MapPut<CombinedValidationEndpoint>("/combination");
    }
}
