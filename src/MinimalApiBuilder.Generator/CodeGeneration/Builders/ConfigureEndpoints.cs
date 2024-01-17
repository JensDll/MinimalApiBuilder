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

        int arity = configures[0].Arity;

        string[] args = new string[arity];
        string[] names = new string[arity];

        for (int i = 0; i < arity; ++i)
        {
            args[i] = $"{Fqn.RouteHandlerBuilder} b{i}";
            names[i] = $"b{i}";
        }

        string joinedArgs = string.Join(", ", args);
        string joinedNames = string.Join(", ", names);

        if (configures.Length == 1)
        {
            AddConfigure(configures[0], joinedArgs);
            return;
        }

        AddConfigureWithLookup(configures, joinedArgs, joinedNames);
    }

    private void AddConfigure(ConfigureToGenerate configure, string args)
    {
        using (OpenBlock(Sources.GeneratedCodeAttribute, $"public static void Configure({args})"))
        {
            foreach ((int i, string endpoint) in configure.Endpoints)
            {
                AppendLine($"{endpoint}._auto_generated_Configure(b{i});");
                AppendLine($"{endpoint}.Configure(b{i});");
            }
        }
    }

    private void AddConfigureWithLookup(ImmutableArray<ConfigureToGenerate> configures, string args, string names)
    {
        string map = $"s_map{_mapCount++}";

        using (OpenBlockExtra(";", Sources.GeneratedCodeAttribute,
            $"private static readonly {Fqn.Dictionary}<(string, int), {Fqn.Action}<{Fqn.RouteHandlerBuilder}[]>> {map} = new()"))
        {
            foreach (ConfigureToGenerate configure in configures)
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
            $"public static void Configure({args}, [{Fqn.CallerFilePath}] string filePath = \"\", [{Fqn.CallerLineNumber}] int lineNumber = 0)"))
        {
            AppendLine($"var configure = {map}[(filePath, lineNumber)];");
            AppendLine($"configure(new[] {{ {names} }});");
        }
    }
}
