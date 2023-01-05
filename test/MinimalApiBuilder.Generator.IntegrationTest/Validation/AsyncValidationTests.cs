﻿namespace MinimalApiBuilder.Generator.IntegrationTest.Validation;

public class AsyncValidationTests
{
    [TestCase]
    public Task Single_Async_Validation()
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
        RuleFor(x => x.Value).MustAsync(async (value, _) => true);
    }
}
";

        return TestHelper.Verify(source);
    }

    [TestCase]
    public Task Multiple_Async_Validation()
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
        RuleFor(x => x.Value).MustAsync(async (value, _) => true);
    }
}
";

        return TestHelper.Verify(source);
    }
}
