using MinimalApiBuilder;

namespace Sample.WebApi.Features.Validation.Sync;

public partial class SyncValidationSingleEndpoint : MinimalApiBuilderEndpoint
{
    private static IResult Handle(SyncValidationRequest request, SyncValidationSingleEndpoint endpoint)
    {
        return Results.Ok();
    }

    public static void Configure(RouteHandlerBuilder builder) { }
}

public partial class SyncValidationMultipleEndpoint : MinimalApiBuilderEndpoint
{
    private static IResult Handle(
        SyncValidationRequest request,
        [AsParameters] SyncValidationParameters parameters,
        SyncValidationMultipleEndpoint endpoint)
    {
        return Results.Ok();
    }

    public static void Configure(RouteHandlerBuilder builder) { }
}
