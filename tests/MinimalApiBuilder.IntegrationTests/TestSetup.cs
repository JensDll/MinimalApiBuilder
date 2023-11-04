using NUnit.Framework;

namespace MinimalApiBuilder.IntegrationTests;

[SetUpFixture]
public class TestSetup
{
    private static TestWebApplicationFactory<Program> s_factory = null!;

    internal static HttpClient Client = null!;

    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        s_factory = new TestWebApplicationFactory<Program>();
        Client = s_factory.CreateClient();
    }

    [OneTimeTearDown]
    public void RunAfterAnyTests()
    {
        s_factory.Dispose();
        Client.Dispose();
    }
}
