using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MinimalApiBuilder.Generator.UnitTests;

public class TestAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
{
    public TestAnalyzerConfigOptionsProvider(AnalyzerConfigOptions globalOptions, string snapshotFolder)
    {
        GlobalOptions = globalOptions;
        SnapshotFolder = Path.Join("__snapshots__", snapshotFolder);
    }

    public string SnapshotFolder { get; }

    public override AnalyzerConfigOptions GlobalOptions { get; }

    public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
    {
        return GlobalOptions;
    }

    public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
    {
        throw new NotImplementedException();
    }
}
