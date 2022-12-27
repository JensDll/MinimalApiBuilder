using MinimalApiBuilder;

namespace Sample.WebApi.Features.Validation.Sync;

public partial class ValidationSync : MinimalApiBuilderEndpoint
{
    private static IResult Handle(ValidationSync endpoint)
    {
        return Results.Ok();
    }

    public static void Configure(RouteHandlerBuilder builder) { }
}
