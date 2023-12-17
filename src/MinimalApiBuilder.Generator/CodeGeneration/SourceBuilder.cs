using System.Text;
using Microsoft.CodeAnalysis;
using MinimalApiBuilder.Generator.Common;
using MinimalApiBuilder.Generator.Entities;

namespace MinimalApiBuilder.Generator.CodeGeneration;

internal abstract class SourceBuilder
{
    private const int IndentSize = 4;
    private readonly StringBuilder _builder = new(Sources.Header);
    private int _indent;

    protected SourceBuilder() : this(default) { }

    protected SourceBuilder(GeneratorOptions options)
    {
        Options = options;
        _builder.AppendLine();
    }

    protected GeneratorOptions Options { get; set; }

    protected ICollection<Diagnostic> Diagnostics { get; } = new List<Diagnostic>();

    public sealed override string ToString() => _builder.ToString();

    public abstract void AddSource(SourceProductionContext context);

    protected void AddSource(SourceProductionContext context, string hintName)
    {
        context.AddSource($"{hintName}.g.cs", ToString());
        foreach (Diagnostic diagnostic in Diagnostics)
        {
            context.ReportDiagnostic(diagnostic);
        }
    }

    protected IDisposable OpenBlock(string value)
    {
        WriteBlockStart(value);
        return new Disposable(() =>
        {
            DecreaseIndent();
            AppendLine("}");
        });
    }

    protected IDisposable OpenBlock(params string[] values)
    {
        for (int i = 0; i < values.Length - 1; ++i)
        {
            AppendLine(values[i]);
        }

        return OpenBlock(values[values.Length - 1]);
    }

    protected IDisposable OpenBlockExtra(string afterClose, string value)
    {
        WriteBlockStart(value);
        return new Disposable(() =>
        {
            DecreaseIndent();
            AppendLine($"}}{afterClose}");
        });
    }

    protected IDisposable OpenBlockExtra(string afterClose, params string[] values)
    {
        for (int i = 0; i < values.Length - 1; ++i)
        {
            AppendLine(values[i]);
        }

        return OpenBlockExtra(afterClose, values[values.Length - 1]);
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
