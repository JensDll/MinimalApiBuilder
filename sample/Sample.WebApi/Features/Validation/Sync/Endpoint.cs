using MinimalApiBuilder;

namespace Sample.WebApi.Features.Validation.Sync;

public partial class ValidationSync : Endpoint<ValidationSync>
{
    private static IResult Handle(ValidationSync endpoint)
    {
        return Results.Ok();
    }
}
