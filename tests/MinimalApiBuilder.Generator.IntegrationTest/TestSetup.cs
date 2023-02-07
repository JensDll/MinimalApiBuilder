namespace MinimalApiBuilder.Generator.IntegrationTest;

[SetUpFixture]
internal class TestContext
{
    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        VerifySourceGenerators.Enable();
    }
}
