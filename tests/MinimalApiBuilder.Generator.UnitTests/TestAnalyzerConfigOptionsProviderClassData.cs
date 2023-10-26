using System.Collections;
using MinimalApiBuilder.Generator.Entities;

namespace MinimalApiBuilder.Generator.UnitTests;

public class TestAnalyzerConfigOptionsProviderClassData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            new TestAnalyzerConfigOptionsProvider(new TestAnalyzerConfigOptions
                {
                    Options =
                    {
                        [GeneratorOptions.Keys.AssignNameToEndpointBuildProperty] = "true"
                    }
                }, $"{GeneratorOptions.Keys.AssignNameToEndpointBuildProperty}=true")
        };

        yield return new object[]
        {
            new TestAnalyzerConfigOptionsProvider(new TestAnalyzerConfigOptions
                {
                    Options =
                    {
                        [GeneratorOptions.Keys.AssignNameToEndpointBuildProperty] = "false"
                    }
                }, $"{GeneratorOptions.Keys.AssignNameToEndpointBuildProperty}=false")
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
