using MinimalApiBuilder;

namespace Fixture.TestApi.Features.Validation.Combination;

public partial class CombinedValidationEndpoint : MinimalApiBuilderEndpoint
{
    private static IResult Handle(Asynchronous.Request request,
        [AsParameters] Synchronous.Parameters parameters,
        CombinedValidationEndpoint endpoint)
    {
        return Results.Ok();
    }

    public static void Configure(RouteHandlerBuilder builder) { }
}
