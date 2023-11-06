using MinimalApiBuilder.Generator.Entities;

namespace MinimalApiBuilder.Generator.UnitTests.Tests;

public class ConfigurationTests : GeneratorUnitTest
{
    private static IEnumerable<TestAnalyzerConfigOptionsProvider> AssignNameProviders()
    {
        yield return new TestAnalyzerConfigOptionsProvider(
            globalOptions: new TestAnalyzerConfigOptions
            {
                Options =
                {
                    [GeneratorOptions.Keys.AssignNameToEndpointBuildProperty] = "true"
                }
            },
            localOptions: new TestAnalyzerConfigOptions(),
            friendlyName: "assign_name_global_true");

        yield return new TestAnalyzerConfigOptionsProvider(
            globalOptions: new TestAnalyzerConfigOptions(),
            localOptions: new TestAnalyzerConfigOptions
            {
                Options =
                {
                    [GeneratorOptions.Keys.AssignNameToEndpoint] = "true"
                }
            },
            friendlyName: "assign_name_local_true");

        yield return new TestAnalyzerConfigOptionsProvider(
            globalOptions: new TestAnalyzerConfigOptions
            {
                Options =
                {
                    [GeneratorOptions.Keys.AssignNameToEndpointBuildProperty] = "true"
                }
            },
            localOptions: new TestAnalyzerConfigOptions
            {
                Options =
                {
                    [GeneratorOptions.Keys.AssignNameToEndpoint] = "false"
                }
            },
            friendlyName: "assign_name_global_true_local_false");
    }

    [Test]
    public async Task With_Assign_Name(
        [ValueSource(nameof(AssignNameProviders))]
        TestAnalyzerConfigOptionsProvider provider)
    {
        // lang=cs
        const string source = """
public partial class E : MinimalApiBuilderEndpoint
{
    public static int Handle(E e) => 0;
}
""";

        await VerifyGeneratorAsync(source, provider);
    }
}
