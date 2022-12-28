using MinimalApiBuilder;

namespace Sample.WebApi.Features.Validation.Sync;

public partial class ValidationSync : MinimalApiBuilderEndpoint
{
    private static IResult Handle(SyncRequest request, ValidationSync endpoint)
    {
        return Results.Ok(request);
    }

    public static void Configure(RouteHandlerBuilder builder) { }
}
