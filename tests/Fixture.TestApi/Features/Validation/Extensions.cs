using Fixture.TestApi.Features.Validation.Asynchronous;
using Fixture.TestApi.Features.Validation.Combination;
using Fixture.TestApi.Features.Validation.Synchronous;
using MinimalApiBuilder;
using SyncSingle = Fixture.TestApi.Features.Validation.Synchronous.SingleEndpoint;
using AsyncSingle = Fixture.TestApi.Features.Validation.Asynchronous.SingleEndpoint;

namespace Fixture.TestApi.Features.Validation;

public static class Extensions
{
    public static void MapValidationFeatures(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder validation = endpoints.MapGroup("/validation").WithTags("Validation");
        validation.MapPost<SyncSingle>("/sync/single");
        validation.MapPatch<SynchronousMultipleValidationEndpoint>("/sync/multiple");
        validation.MapPost<AsyncSingle>("/async/single");
        validation.MapPatch<AsynchronousMultipleValidationEndpoint>("/async/multiple");
        validation.MapPut<CombinedValidationEndpoint>("/combination");
    }
}
