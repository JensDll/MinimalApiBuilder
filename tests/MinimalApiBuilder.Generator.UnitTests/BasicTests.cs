﻿namespace MinimalApiBuilder.Generator.UnitTests;

[UsesVerify]
public class BasicTests
{
    [Theory]
    [ClassData(typeof(TestAnalyzerConfigOptionsProviderClassData))]
    public Task Single_Endpoint_No_Parameters(TestAnalyzerConfigOptionsProvider provider)
    {
        // lang=cs
        const string source = @"
using MinimalApiBuilder;
using Microsoft.AspNetCore.Builder;

namespace Features;

public partial class Endpoint1 : MinimalApiBuilderEndpoint
{
    private static IResult Handle(Endpoint1 endpoint)
    {
        return Results.Ok();
    }

    public static void Configure(RouteHandlerBuilder builder) { }
}";

        return TestHelper.Verify(source, provider);
    }

    [Theory]
    [ClassData(typeof(TestAnalyzerConfigOptionsProviderClassData))]
    public Task Multiple_Endpoints_No_Parameters(TestAnalyzerConfigOptionsProvider provider)
    {
        // lang=cs
        const string source = @"
using MinimalApiBuilder;
using Microsoft.AspNetCore.Builder;

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
    private static IResult HandleAsync(Endpoint2 endpoint)
    {
        return Results.Ok();
    }

    public static void Configure(RouteHandlerBuilder builder) { }
}";

        return TestHelper.Verify(source, provider);
    }

    [Theory]
    [ClassData(typeof(TestAnalyzerConfigOptionsProviderClassData))]
    public Task Without_Configure(TestAnalyzerConfigOptionsProvider provider)
    {
        // lang=cs
        const string source = @"
using MinimalApiBuilder;

namespace Features;

public partial class WithoutConfigureEndpoint : MinimalApiBuilderEndpoint
{
    private static int Handle(WithoutConfigureEndpoint endpoint)
    {
        return 42;
    }
}";

        return TestHelper.Verify(source, provider);
    }

    [Theory]
    [ClassData(typeof(TestAnalyzerConfigOptionsProviderClassData))]
    public Task Global_Namespace(TestAnalyzerConfigOptionsProvider provider)
    {
        // lang=cs
        const string source = @"
using MinimalApiBuilder;

public partial class GlobalNamespaceEndpoint : MinimalApiBuilderEndpoint
{
    private static int Handle(GlobalNamespaceEndpoint endpoint)
    {
        return 42;
    }
}";

        return TestHelper.Verify(source, provider);
    }
}
