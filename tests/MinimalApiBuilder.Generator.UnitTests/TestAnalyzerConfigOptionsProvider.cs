﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MinimalApiBuilder.Generator.UnitTests;

public class TestAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
{
    private readonly AnalyzerConfigOptions _localOptions;
    private readonly string _friendlyName;

    public TestAnalyzerConfigOptionsProvider(
        AnalyzerConfigOptions globalOptions,
        AnalyzerConfigOptions localOptions,
        string friendlyName)
    {
        _localOptions = localOptions;
        _friendlyName = friendlyName;
        GlobalOptions = globalOptions;
    }

    public override AnalyzerConfigOptions GlobalOptions { get; }

    public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
    {
        return _localOptions;
    }

    public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
    {
        throw new NotImplementedException();
    }

    public sealed override string ToString()
    {
        return _friendlyName;
    }
}
