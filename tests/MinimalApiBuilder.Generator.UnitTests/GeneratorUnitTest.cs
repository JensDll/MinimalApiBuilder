using System.Collections;
using System.Collections.Immutable;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.DependencyInjection;

namespace MinimalApiBuilder.Generator.UnitTests;

internal abstract class GeneratorUnitTest
{
    private static readonly CSharpParseOptions s_parseOptions = new(LanguageVersion.CSharp11);

    private static readonly string s_dllDirectory =
        Path.GetDirectoryName(typeof(object).Assembly.Location)
        ?? throw new DirectoryNotFoundException("Cannot find object assembly directory");

    private static readonly MetadataReference[] s_metadataReferences =
    {
        // NetCore
        MetadataReference.CreateFromFile(typeof(Attribute).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(HttpStatusCode).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(IServiceProvider).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(ICollection).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(System.Linq.Expressions.Expression).Assembly.Location),
        MetadataReference.CreateFromFile(Path.Join(s_dllDirectory, "System.Runtime.dll")),
        MetadataReference.CreateFromFile(Path.Join(s_dllDirectory, "System.Collections.dll")),
        // AspNetCore
        MetadataReference.CreateFromFile(typeof(RouteAttribute).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(RouteHandlerBuilder).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(ServiceLifetime).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(HttpContext).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(TypedResults).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(RouteData).Assembly.Location),
        // MinimalApiBuilder
        MetadataReference.CreateFromFile(typeof(MinimalApiBuilderEndpoint).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(AbstractValidator<>).Assembly.Location)
    };

    private static readonly ImmutableArray<DiagnosticAnalyzer> s_analyzers =
        Directory.GetFiles("copied-analyzers", "*.dll")
            .Select(Assembly.LoadFrom)
            .SelectMany(static assembly => assembly.GetTypes())
            .Where(static type => type.GetCustomAttribute<DiagnosticAnalyzerAttribute>() is not null)
            .Select(static type => Unsafe.As<DiagnosticAnalyzer>(Activator.CreateInstance(type))!)
            .ToImmutableArray();

    private static readonly Dictionary<string, ReportDiagnostic> s_diagnosticsOptions =
        s_analyzers
            .SelectMany(static analyzer => analyzer.SupportedDiagnostics)
            .Select(descriptor => descriptor.Id)
            .Distinct()
            .Where(static id => id.StartsWith("CA", StringComparison.Ordinal))
            .ToDictionary(static id => id, _ => ReportDiagnostic.Warn)
            .AddAndReturn("CS1701", ReportDiagnostic.Suppress)
            .ChangeAndReturn("CA1014", ReportDiagnostic.Suppress) // CA1014: Mark assemblies with CLSCompliantAttribute
            .ChangeAndReturn("CA1016",
                ReportDiagnostic.Suppress) // CA1016: Mark assemblies with AssemblyVersionAttribute
            .ChangeAndReturn("CA1017", ReportDiagnostic.Suppress) // CA1017: Mark assemblies with ComVisibleAttribute
            .ChangeAndReturn("CA1050", ReportDiagnostic.Suppress) // CA1050: Declare types in namespaces
            .ChangeAndReturn("CA1812", ReportDiagnostic.Suppress) // CA1812: Avoid uninstantiated internal classes
            .ChangeAndReturn("CA2007", ReportDiagnostic.Suppress); // CA2007: Do not directly await a Task

    private static readonly CSharpCompilationOptions s_compilationOptions =
        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            .WithNullableContextOptions(NullableContextOptions.Enable)
            .WithSpecificDiagnosticOptions(s_diagnosticsOptions);

    protected static Task VerifyGeneratorAsync(string source)
    {
        return VerifyGeneratorAsync(source, TestAnalyzerConfigOptionsProvider.Default);
    }

    protected static Task VerifyGeneratorAsync(string source, string mapActions)
    {
        return VerifyGeneratorAsync(source, mapActions, TestAnalyzerConfigOptionsProvider.Default);
    }

    protected static Task VerifyGeneratorAsync(
        string source,
        TestAnalyzerConfigOptionsProvider optionsProvider)
    {
        return VerifyGeneratorAsyncImpl(GetSource(source), optionsProvider);
    }

    protected static Task VerifyGeneratorAsync(
        string source,
        string mapActions,
        TestAnalyzerConfigOptionsProvider optionsProvider)
    {
        return VerifyGeneratorAsyncImpl(GetSource(source, mapActions), optionsProvider);
    }

    private static async Task VerifyGeneratorAsyncImpl(string source, AnalyzerConfigOptionsProvider optionsProvider)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

        var compilation = CSharpCompilation.Create(
            assemblyName: nameof(GeneratorUnitTest) + "Assembly",
            syntaxTrees: new[] { syntaxTree },
            references: s_metadataReferences,
            options: s_compilationOptions);

        IIncrementalGenerator generator = new MinimalApiBuilderGenerator();
        IEnumerable<ISourceGenerator> generators = GetSourceGenerators(generator);

        GeneratorDriver driver = CSharpGeneratorDriver
            .Create(generators, optionsProvider: optionsProvider, parseOptions: s_parseOptions)
            .RunGeneratorsAndUpdateCompilation(compilation, out var newCompilation, out _);

        await Task.WhenAll(
            Task.Run(() => AssertCompilation(newCompilation)),
            AssertCompilationWithAnalyzersAsync(newCompilation),
            Verify(driver).DisableDiff());
    }

    private static IEnumerable<ISourceGenerator> GetSourceGenerators(params IIncrementalGenerator[] generators)
    {
        return generators.Select(GeneratorExtensions.AsSourceGenerator);
    }

    private static void AssertCompilation(Compilation compilation)
    {
        using MemoryStream output = new();
        EmitResult result = compilation.Emit(output);
        IEnumerable<Diagnostic> warningsOrWorse = WarningsOrWorse(result.Diagnostics);
        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True);
            Assert.That(warningsOrWorse, Is.Empty);
        });
    }

    private static async Task AssertCompilationWithAnalyzersAsync(Compilation compilation)
    {
        compilation = RemoveAnnotationsPreventingCodeAnalysis(compilation);
        CompilationWithAnalyzers withAnalyzers = compilation.WithAnalyzers(s_analyzers);
        ImmutableArray<Diagnostic> diagnostics = await withAnalyzers.GetAnalyzerDiagnosticsAsync();
        IEnumerable<Diagnostic> warningsOrWorse = WarningsOrWorse(diagnostics);
        Assert.That(warningsOrWorse, Is.Empty);
    }

    private static Compilation RemoveAnnotationsPreventingCodeAnalysis(Compilation compilation)
    {
        GeneratedCodeRewriter rewriter = new();

        foreach (SyntaxTree syntaxTree in compilation.SyntaxTrees)
        {
            if (!syntaxTree.FilePath.EndsWith(".g.cs", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            CompilationUnitSyntax root = syntaxTree.GetCompilationUnitRoot().WithoutLeadingTrivia();
            SyntaxNode newSource = rewriter.Visit(root);

            compilation = compilation.ReplaceSyntaxTree(syntaxTree, newSource.SyntaxTree);
        }

        return compilation;
    }

    private static IEnumerable<Diagnostic> WarningsOrWorse(IEnumerable<Diagnostic> diagnostics)
    {
        return diagnostics.Where(static diagnostic => diagnostic.Severity >= DiagnosticSeverity.Warning);
    }

    private static string GetSource(string source)
    {
        // lang=cs
        return $"""
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MinimalApiBuilder;
using FluentValidation;
using System;
using System.Reflection;
using System.Threading.Tasks;

{source}
""";
    }

    private static string GetSource(string source, string mapActions)
    {
        // lang=cs
        return $$"""
{{GetSource(source)}}

public static class TestMapActions
{
    public static IEndpointRouteBuilder MapTestEndpoints(this IEndpointRouteBuilder app)
    {
        {{mapActions}}
        return app;
    }
}
""";
    }
}
