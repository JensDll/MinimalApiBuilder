using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using MinimalApiBuilder;

namespace Fixture.TestApi.Features.Validation.Sync;

public partial class SyncSingleEndpoint : MinimalApiBuilderEndpoint
{
    private static IResult Handle(Request request, SyncSingleEndpoint endpoint)
    {
        return Results.Ok();
    }

    public static void Configure(RouteHandlerBuilder builder) { }
}

public partial class SyncMultipleEndpoint : MinimalApiBuilderEndpoint
{
    private static IResult Handle(
        Request request,
        [AsParameters] Parameters parameters,
        SyncMultipleEndpoint endpoint)
    {
        return Results.Ok();
    }

    public static void Configure(RouteHandlerBuilder builder) { }
}
