using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Fixture.TestApi.Features.Validation;

public static class FeatureExtensions
{
    public static void MapValidationFeatures(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder validation = endpoints.MapGroup("/validation").WithTags("Validation");
        // validation.MapPost<SyncSingleValidationEndpoint>("/sync/single");
        // validation.MapPatch<SyncMultipleValidationEndpoint>("/sync/multiple");
        // validation.MapPost<AsyncSingleValidationEndpoint>("/async/single");
        // validation.MapPatch<AsyncMultipleValidationEndpoint>("/async/multiple");
        // validation.MapPut<CombinedValidationEndpoint>("/combination");
    }
}
