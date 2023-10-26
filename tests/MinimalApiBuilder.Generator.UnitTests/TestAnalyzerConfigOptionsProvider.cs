using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MinimalApiBuilder.Generator.UnitTests;

public class TestAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
{
    private readonly AnalyzerConfigOptions _localOptions;
    private readonly string _snapshotFolder;

    public TestAnalyzerConfigOptionsProvider(AnalyzerConfigOptions globalOptions,
        AnalyzerConfigOptions localOptions,
        string snapshotFolder)
    {
        _localOptions = localOptions;
        _snapshotFolder = snapshotFolder;
        GlobalOptions = globalOptions;
        SnapshotFolder = Path.Join("__snapshots__", snapshotFolder);
    }

    public string SnapshotFolder { get; }

    public override AnalyzerConfigOptions GlobalOptions { get; }

    public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
    {
        return _localOptions;
    }

    public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return _snapshotFolder;
    }
}
