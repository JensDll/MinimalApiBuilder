using MinimalApiBuilder;

namespace Fixture.TestApi.Features.Validation.Synchronous;

public partial class SingleEndpoint : MinimalApiBuilderEndpoint
{
    private static IResult Handle(Request request, SingleEndpoint endpoint)
    {
        return Results.Ok();
    }

    public static void Configure(RouteHandlerBuilder builder) { }
}

public partial class SynchronousMultipleValidationEndpoint : MinimalApiBuilderEndpoint
{
    private static IResult Handle(
        Request request,
        [AsParameters] Parameters parameters,
        SynchronousMultipleValidationEndpoint endpoint)
    {
        return Results.Ok();
    }

    public static void Configure(RouteHandlerBuilder builder) { }
}
