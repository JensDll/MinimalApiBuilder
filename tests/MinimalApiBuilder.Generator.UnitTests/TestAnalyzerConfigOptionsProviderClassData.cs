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
                        [$"build_property.{GeneratorOptions.Keys.AssignNameToEndpoint}"] = "true"
                    }
                }, $"build_property.{GeneratorOptions.Keys.AssignNameToEndpoint}=true")
        };

        yield return new object[]
        {
            new TestAnalyzerConfigOptionsProvider(new TestAnalyzerConfigOptions
                {
                    Options =
                    {
                        [$"build_property.{GeneratorOptions.Keys.AssignNameToEndpoint}"] = "false"
                    }
                }, $"build_property.{GeneratorOptions.Keys.AssignNameToEndpoint}=false")
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
