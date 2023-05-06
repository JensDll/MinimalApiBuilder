namespace MinimalApiBuilder.Generator.IntegrationTest;

[SetUpFixture]
public class TestSetup
{
    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        VerifySourceGenerators.Initialize();
    }
}
