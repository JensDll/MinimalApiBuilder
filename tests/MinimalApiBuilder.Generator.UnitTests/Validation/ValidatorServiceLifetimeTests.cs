namespace MinimalApiBuilder.Generator.UnitTests;

[UsesVerify]
public class ValidatorServiceLifetimeTests
{
    [Theory]
    [ClassData(typeof(TestAnalyzerConfigOptionsProviderClassData))]
    public Task Default(TestAnalyzerConfigOptionsProvider provider)
    {
        // lang=cs
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
        // lang=cs
        const string source = @"
using MinimalApiBuilder;
using Microsoft.Extensions.DependencyInjection;
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
        // lang=cs
        const string source = @"
using MinimalApiBuilder;
using Microsoft.Extensions.DependencyInjection;
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
        // lang=cs
        const string source = @"
using MinimalApiBuilder;
using Microsoft.Extensions.DependencyInjection;
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

    [Theory]
    [ClassData(typeof(TestAnalyzerConfigOptionsProviderClassData))]
    public Task Multiple_Attributes(TestAnalyzerConfigOptionsProvider provider)
    {
        // lang=cs
        const string source = @"
using MinimalApiBuilder;
using Microsoft.Extensions.DependencyInjection;
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

[A1]
[A2, RegisterValidator(ServiceLifetime.Scoped)]
public class Validator : AbstractValidator<Request>
{
    public Validator() { }
}";

        return TestHelper.Verify(source, provider);
    }
}
