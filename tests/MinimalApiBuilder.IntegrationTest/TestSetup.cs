using NUnit.Framework;

namespace MinimalApiBuilder.IntegrationTest;

[SetUpFixture]
public class TestSetup
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
