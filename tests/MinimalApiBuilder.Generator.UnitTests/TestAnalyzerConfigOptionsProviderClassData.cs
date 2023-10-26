using System.Collections;
using MinimalApiBuilder.Generator.Entities;

namespace MinimalApiBuilder.Generator.UnitTests;

public class TestAnalyzerConfigOptionsProviderClassData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            new TestAnalyzerConfigOptionsProvider(
                globalOptions: new TestAnalyzerConfigOptions
                {
                    Options =
                    {
                        [GeneratorOptions.Keys.AssignNameToEndpointBuildProperty] = "true"
                    }
                },
                localOptions: new TestAnalyzerConfigOptions(),
                snapshotFolder: "assign_name_global_true")
        };

        yield return new object[]
        {
            new TestAnalyzerConfigOptionsProvider(
                globalOptions: new TestAnalyzerConfigOptions(),
                localOptions: new TestAnalyzerConfigOptions
                {
                    Options =
                    {
                        [GeneratorOptions.Keys.AssignNameToEndpoint] = "true"
                    }
                },
                snapshotFolder: "assign_name_local_true")
        };

        yield return new object[]
        {
            new TestAnalyzerConfigOptionsProvider(
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
                snapshotFolder: "assign_name_global_true_local_false")
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
