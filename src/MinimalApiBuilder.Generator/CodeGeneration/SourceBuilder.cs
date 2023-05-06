﻿using System.Text;
using Microsoft.CodeAnalysis;
using MinimalApiBuilder.Generator.Entities;

namespace MinimalApiBuilder.Generator.CodeGeneration;

internal abstract class SourceBuilder
{
    private const int IndentSize = 4;
    private readonly StringBuilder _builder = new(@"#nullable enable

// <auto-generated>
// This is a MinimalApiBuilder source generated file.
// </auto-generated>

");
    private int _indent;

    protected SourceBuilder(GeneratorOptions options, params string[] usingStatements) : this(options)
    {
        foreach (string usingStatement in usingStatements)
        {
            _builder.AppendLine($"using {usingStatement};");
        }

        _builder.AppendLine();
    }

    protected SourceBuilder(GeneratorOptions options)
    {
        Options = options;
    }

    public bool IsAdded { get; protected set; }

    protected GeneratorOptions Options { get; }

    public abstract void AddSource(SourceProductionContext context);

    public sealed override string ToString() => _builder.ToString();

    protected IDisposable OpenBlock(string value)
    {
        WriteBlockStart(value);
        return new Disposable(() =>
        {
            DecreaseIndent();
            AppendLine("}");
        });
    }

    protected IDisposable OpenBlock(string value, string afterClose)
    {
        WriteBlockStart(value);
        return new Disposable(() =>
        {
            DecreaseIndent();
            AppendLine($"}}{afterClose}");
        });
    }

    protected void AppendLine(string value)
    {
        _builder.Append(' ', _indent);
        _builder.AppendLine(value);
    }

    private void IncreaseIndent()
    {
        _indent += IndentSize;
    }

    private void DecreaseIndent()
    {
        _indent -= IndentSize;
    }

    private void WriteBlockStart(string value)
    {
        _builder.Append(' ', _indent);
        _builder.AppendLine(value);
        _builder.Append(' ', _indent);
        _builder.AppendLine("{");
        IncreaseIndent();
    }
}
