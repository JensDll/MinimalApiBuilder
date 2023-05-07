namespace MinimalApiBuilder.Generator.UnitTests;

[UsesVerify]
public class SyncValidationTests
{
    [Theory]
    [ClassData(typeof(TestAnalyzerConfigOptionsProviderClassData))]
    public Task Single_Sync_Validation(TestAnalyzerConfigOptionsProvider provider)
    {
        const string source = @"
using MinimalApiBuilder;
using FluentValidation;

namespace Features;

public partial class Endpoint1 : MinimalApiBuilderEndpoint
{
    private static IResult Handle(Endpoint1 endpoint, Request request)
    {
        return Results.Ok();
    }

    public static void Configure(RouteHandlerBuilder builder) { }
}

public class Request
{
    public string Value { get; set; }
}

public class Validator : AbstractValidator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Value).NotEmpty();
    }
}
";

        return TestHelper.Verify(source, provider);
    }

    [Theory]
    [ClassData(typeof(TestAnalyzerConfigOptionsProviderClassData))]
    public Task Multiple_Sync_Validation(TestAnalyzerConfigOptionsProvider provider)
    {
        const string source = @"
using MinimalApiBuilder;
using FluentValidation;

namespace Features;

public partial class Endpoint1 : MinimalApiBuilderEndpoint
{
    private static IResult Handle(Endpoint1 endpoint, Request request1, Request request2)
    {
        return Results.Ok();
    }

    public static void Configure(RouteHandlerBuilder builder) { }
}

public class Request
{
    public string Value { get; set; }
}

public class Validator : AbstractValidator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Value).NotEmpty();
    }
}
";

        return TestHelper.Verify(source, provider);
    }
}
