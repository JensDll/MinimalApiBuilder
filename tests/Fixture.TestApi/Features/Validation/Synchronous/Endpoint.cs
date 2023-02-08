using MinimalApiBuilder;

namespace Fixture.TestApi.Features.Validation.Synchronous;

public partial class EndpointSingle : MinimalApiBuilderEndpoint
{
    private static IResult Handle(Request request, EndpointSingle endpoint)
    {
        return Results.Ok();
    }

    public static void Configure(RouteHandlerBuilder builder) { }
}

public partial class EndpointMultiple : MinimalApiBuilderEndpoint
{
    private static IResult Handle(
        Request request,
        [AsParameters] Parameters parameters,
        EndpointMultiple endpoint)
    {
        return Results.Ok();
    }

    public static void Configure(RouteHandlerBuilder builder) { }
}
