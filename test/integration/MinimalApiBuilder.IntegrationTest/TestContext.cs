using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace MinimalApiBuilder.IntegrationTest;

[SetUpFixture]
public class TestContext
{
    internal static TestWebApplicationFactory<Program> Factory = null!;
    internal static HttpClient Client = null!;

    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        Factory = new TestWebApplicationFactory<Program>();
        Client = Factory.CreateClient();
    }

    [OneTimeTearDown]
    public void RunAfterAnyTests()
    {
        Factory.Dispose();
        Client.Dispose();
    }
}

public class TestWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services => { });
        return base.CreateHost(builder);
    }
}

public class ErrorResult
{
    public required int StatusCode { get; init; }

    public required string Message { get; init; }

    public required string[] Errors { get; init; }
}
