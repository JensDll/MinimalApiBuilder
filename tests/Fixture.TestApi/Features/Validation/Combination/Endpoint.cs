using MinimalApiBuilder;
using Fixture.TestApi.Features.Validation;

namespace Fixture.TestApi.Features.Validation.Combination;

public partial class Endpoint : MinimalApiBuilderEndpoint
{
    private static IResult Handle(Asynchronous.Request request,
        [AsParameters] Synchronous.Parameters parameters,
        Endpoint endpoint)
    {
        return Results.Ok();
    }

    public static void Configure(RouteHandlerBuilder builder) { }
}
