namespace MinimalApiBuilder.Generator.IntegrationTest;

public class BasicTests
{
    [TestCase]
    public Task Single_Endpoint_No_Parameters()
    {
        const string source = @"
using MinimalApiBuilder;

namespace Features;

public partial class Endpoint1 : MinimalApiBuilderEndpoint
{
    private static IResult Handle(Endpoint1 endpoint)
    {
        return Results.Ok();
    }

    public static void Configure(RouteHandlerBuilder builder) { }
}";

        return TestHelper.Verify(source);
    }

    [TestCase]
    public Task Multiple_Endpoints_No_Parameters()
    {
        const string source = @"
using MinimalApiBuilder;

namespace Features;

public partial class Endpoint1 : MinimalApiBuilderEndpoint
{
    private static IResult Handle(Endpoint1 endpoint)
    {
        return Results.Ok();
    }

    public static void Configure(RouteHandlerBuilder builder) { }
}

public partial class Endpoint2 : MinimalApiBuilderEndpoint
{
    private static IResult Handle(Endpoint2 endpoint)
    {
        return Results.Ok();
    }

    public static void Configure(RouteHandlerBuilder builder) { }
}";

        return TestHelper.Verify(source);
    }
}
