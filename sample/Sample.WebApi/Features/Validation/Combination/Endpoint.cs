using MinimalApiBuilder;
using Sample.WebApi.Features.Validation.Async;
using Sample.WebApi.Features.Validation.Sync;

namespace Sample.WebApi.Features.Validation.Combination;

public partial class SyncAsyncValidationEndpoint : MinimalApiBuilderEndpoint
{
    private static IResult Handle(AsyncValidationRequest request,
        [AsParameters] SyncValidationParameters parameters,
        SyncAsyncValidationEndpoint endpoint)
    {
        return Results.Ok();
    }

    public static void Configure(RouteHandlerBuilder builder) { }
}
