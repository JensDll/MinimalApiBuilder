using MinimalApiBuilder;

namespace Sample.WebApi.Features.Validation.Sync;

public class ValidationSync : Endpoint<ValidationSync>
{
    public static IResult Handle(ValidationSync endpoint)
    {
        return Results.Ok();
    }
}
