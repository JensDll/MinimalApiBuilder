using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using MinimalApiBuilder.Generator.Common;
using MinimalApiBuilder.Generator.Entities;

namespace MinimalApiBuilder.Generator.CodeGeneration.Builders;

internal sealed class ConfigureEndpoints : SourceBuilder
{
    private readonly IDisposable _namespace;
    private readonly IDisposable _class;
    private int _mapCount;

    public ConfigureEndpoints()
    {
        _namespace = OpenBlock("namespace MinimalApiBuilder.Generator");
        _class = OpenBlock($"internal static partial class {nameof(ConfigureEndpoints)}");
    }

    public override void AddSource(SourceProductionContext context)
    {
        _class.Dispose();
        _namespace.Dispose();
        base.AddSource(context, $"{nameof(ConfigureEndpoints)}.Specialization");
    }

    public void Add(ImmutableArray<ConfigureToGenerate> configures)
    {
        if (configures.Length == 0)
        {
            return;
        }

        string builders = string.Join(", ", Enumerable.Range(0, configures[0].Arity)
            .Select(static i => $"{Fqn.RouteHandlerBuilder} b{i}"));

        if (configures.Length == 1)
        {
            AddConfigure(configures[0], builders);
            return;
        }

        AddConfigureWithLookup(configures, builders);
    }

    private void AddConfigure(ConfigureToGenerate configure, string builders)
    {
        using (OpenBlock(Sources.GeneratedCodeAttribute, $"public static void Configure({builders})"))
        {
            foreach ((int i, string endpoint) in configure.Endpoints)
            {
                AppendLine($"{endpoint}._auto_generated_Configure(b{i});");
                AppendLine($"{endpoint}.Configure(b{i});");
            }
        }
    }

    private void AddConfigureWithLookup(ImmutableArray<ConfigureToGenerate> configures, string builders)
    {
        string map = $"s_map{_mapCount++}";

        using (OpenBlockExtra(";", Sources.GeneratedCodeAttribute,
            $"private static readonly {Fqn.Dictionary}<(string, int), {Fqn.Action}<{Fqn.RouteHandlerBuilder}[]>> {map} = new()"))
        {
            foreach (var configure in configures)
            {
                using (OpenBlockExtra(",",
                    $"[(@\"{configure.FilePath}\", {configure.LineNumber})] = static ({Fqn.RouteHandlerBuilder}[] builders) =>"))
                {
                    foreach ((int i, string endpoint) in configure.Endpoints)
                    {
                        AppendLine($"{endpoint}._auto_generated_Configure(builders[{i}]);");
                        AppendLine($"{endpoint}.Configure(builders[{i}]);");
                    }
                }
            }
        }

        using (OpenBlock(Sources.GeneratedCodeAttribute,
            $"public static void Configure({builders}, [{Fqn.CallerFilePath}] string filePath = \"\", [{Fqn.CallerLineNumber}] int lineNumber = 0)"))
        {
            AppendLine($"var configure = {map}[(filePath, lineNumber)];");
            string args = string.Join(", ", Enumerable.Range(0, configures[0].Arity).Select(static i => $"b{i}"));
            AppendLine($"configure(new[] {{ {args} }});");
        }
    }
}
