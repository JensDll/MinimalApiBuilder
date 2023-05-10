using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using MinimalApiBuilder;

namespace Fixture.TestApi.Features.Validation.Combination;

public partial class CombinedEndpoint : MinimalApiBuilderEndpoint
{
    private static IResult Handle(Async.Request request,
        [AsParameters] Sync.Parameters parameters,
        CombinedEndpoint endpoint)
    {
        return Results.Ok();
    }

    public static void Configure(RouteHandlerBuilder builder) { }
}
