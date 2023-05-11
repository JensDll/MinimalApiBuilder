namespace MinimalApiBuilder.Generator.UnitTests;

[UsesVerify]
public class ServiceLifetimeTests
{
    [Theory]
    [ClassData(typeof(TestAnalyzerConfigOptionsProviderClassData))]
    public Task Default(TestAnalyzerConfigOptionsProvider provider)
    {
        const string source = @"
using MinimalApiBuilder;
using Microsoft.AspNetCore.Builder;
using FluentValidation;

namespace Features;

public partial class Endpoint1 : MinimalApiBuilderEndpoint
{
    private static IResult Handle(Endpoint1 endpoint, Request request)
    {
        return Results.Ok();
    }
}

public class Request
{
    public string Value { get; set; }
}

public class Validator : AbstractValidator<Request>
{
    public Validator() { }
}";

        return TestHelper.Verify(source, provider);
    }

    [Theory]
    [ClassData(typeof(TestAnalyzerConfigOptionsProviderClassData))]
    public Task Singleton(TestAnalyzerConfigOptionsProvider provider)
    {
        const string source = @"
using MinimalApiBuilder;
using Microsoft.AspNetCore.Builder;
using FluentValidation;

namespace Features;

public partial class Endpoint1 : MinimalApiBuilderEndpoint
{
    private static IResult Handle(Endpoint1 endpoint, Request request)
    {
        return Results.Ok();
    }
}

public class Request
{
    public string Value { get; set; }
}

[RegisterValidator(ServiceLifetime.Singleton)]
public class Validator : AbstractValidator<Request>
{
    public Validator() { }
}";

        return TestHelper.Verify(source, provider);
    }

    [Theory]
    [ClassData(typeof(TestAnalyzerConfigOptionsProviderClassData))]
    public Task Scoped(TestAnalyzerConfigOptionsProvider provider)
    {
        const string source = @"
using MinimalApiBuilder;
using Microsoft.AspNetCore.Builder;
using FluentValidation;

namespace Features;

public partial class Endpoint1 : MinimalApiBuilderEndpoint
{
    private static IResult Handle(Endpoint1 endpoint, Request request)
    {
        return Results.Ok();
    }
}

public class Request
{
    public string Value { get; set; }
}

[RegisterValidator(ServiceLifetime.Scoped)]
public class Validator : AbstractValidator<Request>
{
    public Validator() { }
}";

        return TestHelper.Verify(source, provider);
    }

    [Theory]
    [ClassData(typeof(TestAnalyzerConfigOptionsProviderClassData))]
    public Task Transient(TestAnalyzerConfigOptionsProvider provider)
    {
        const string source = @"
using MinimalApiBuilder;
using Microsoft.AspNetCore.Builder;
using FluentValidation;

namespace Features;

public partial class Endpoint1 : MinimalApiBuilderEndpoint
{
    private static IResult Handle(Endpoint1 endpoint, Request request)
    {
        return Results.Ok();
    }
}

public class Request
{
    public string Value { get; set; }
}

[RegisterValidator(ServiceLifetime.Transient)]
public class Validator : AbstractValidator<Request>
{
    public Validator() { }
}";

        return TestHelper.Verify(source, provider);
    }
}
