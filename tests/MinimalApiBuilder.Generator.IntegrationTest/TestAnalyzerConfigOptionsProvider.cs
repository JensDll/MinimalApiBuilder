using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using MinimalApiBuilder.Generator.Entities;

namespace MinimalApiBuilder.Generator.IntegrationTest;

public class TestAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
{
    private TestAnalyzerConfigOptionsProvider(AnalyzerConfigOptions globalOptions, string snapshotFolder)
    {
        GlobalOptions = globalOptions;
        SnapshotFolder = $"__snapshots__\\{snapshotFolder}";
    }

    public string SnapshotFolder { get; }

    public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
    {
        throw new NotImplementedException();
    }

    public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
    {
        throw new NotImplementedException();
    }

    public override AnalyzerConfigOptions GlobalOptions { get; }

    public static IEnumerable<TestAnalyzerConfigOptionsProvider> TestCases => new[]
    {
        new TestAnalyzerConfigOptionsProvider(new TestAnalyzerConfigOptions
        {
            Options =
            {
                [$"build_property.{GeneratorOptions.Keys.AssignNameToEndpoint}"] = "true"
            }
        }, $"build_property.{GeneratorOptions.Keys.AssignNameToEndpoint}=true"),
        new TestAnalyzerConfigOptionsProvider(new TestAnalyzerConfigOptions
        {
            Options =
            {
                [$"build_property.{GeneratorOptions.Keys.AssignNameToEndpoint}"] = "false"
            }
        }, $"build_property.{GeneratorOptions.Keys.AssignNameToEndpoint}=false")
    };
}
