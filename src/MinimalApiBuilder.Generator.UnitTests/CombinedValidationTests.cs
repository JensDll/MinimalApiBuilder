namespace MinimalApiBuilder.Generator.UnitTests;

[UsesVerify]
public class CombinedValidationTests
{
    [Theory]
    [ClassData(typeof(TestAnalyzerConfigOptionsProviderClassData))]
    public Task Single_Combined_Validation(TestAnalyzerConfigOptionsProvider provider)
    {
        const string source = @"
using MinimalApiBuilder;
using FluentValidation;

namespace Features;

public partial class Endpoint1 : MinimalApiBuilderEndpoint
{
    private static IResult Handle(Endpoint1 endpoint, Request request, Parameters parameters)
    {
        return Results.Ok();
    }

    public static void Configure(RouteHandlerBuilder builder) { }
}

public class Request
{
    public string Value { get; set; }
}

[Something]
[RegisterValidator(ServiceLifetime.Transient)]
public class SyncValidator : AbstractValidator<Request>
{
    public SyncValidator()
    {
        RuleFor(x => x.Value).NotEmpty();
    }
}

public struct Parameters
{
    public string Value { get; set; }
}

[Something]
[RegisterValidator(ServiceLifetime.Scoped)]
public class AsyncValidator : AbstractValidator<Parameters>
{
    public AsyncValidator()
    {
        RuleFor(x => x.Value).MustAsync((value, _) => Task.FromResult(true));
    }
}
";

        return TestHelper.Verify(source, provider);
    }
}
