using MinimalApiBuilder.IntegrationTests.Infrastructure;
using NUnit.Framework;

// ReSharper disable CheckNamespace

[SetUpFixture]
internal static class TestSetup
{
    private static TestWebApplicationFactory<Program> s_factory = null!;

    internal static HttpClient Client { get; private set; } = null!;

    [OneTimeSetUp]
    public static void RunBeforeAnyTests()
    {
        s_factory = new TestWebApplicationFactory<Program>();
        Client = s_factory.CreateClient();
    }

    [OneTimeTearDown]
    public static void RunAfterAnyTests()
    {
        s_factory.Dispose();
        Client.Dispose();
    }
}
