using System.Collections;
using System.Collections.Immutable;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.DependencyInjection;

namespace MinimalApiBuilder.Generator.UnitTests;

public abstract class GeneratorUnitTest
{
    private static readonly CSharpParseOptions s_parseOptions = new(LanguageVersion.CSharp11);

    private static readonly CSharpCompilationOptions s_compilationOptions =
        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            .WithNullableContextOptions(NullableContextOptions.Enable)
            .WithSpecificDiagnosticOptions(new Dictionary<string, ReportDiagnostic>
            {
                { "CS1701", ReportDiagnostic.Suppress },
                { "CA2012", ReportDiagnostic.Warn }
            });

    private static readonly string s_dllDirectory =
        Path.GetDirectoryName(typeof(object).Assembly.Location)
        ?? throw new InvalidOperationException("Cannot find object assembly directory");

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

    protected static Task VerifyGeneratorAsync(string source)
    {
        TestAnalyzerConfigOptionsProvider optionsProvider = new(
            globalOptions: new TestAnalyzerConfigOptions(),
            localOptions: new TestAnalyzerConfigOptions(),
            friendlyName: "default_configuration");

        return VerifyGeneratorAsync(source, optionsProvider);
    }

    protected static async Task VerifyGeneratorAsync(string source, TestAnalyzerConfigOptionsProvider optionsProvider)
    {
        // lang=cs
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText($"""
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http.HttpResults;
using MinimalApiBuilder;
using FluentValidation;
using System;
using System.Reflection;
using System.Threading.Tasks;

{source}
""");

        var compilation = CSharpCompilation.Create(
            assemblyName: nameof(GeneratorUnitTest),
            syntaxTrees: new[] { syntaxTree },
            references: s_metadataReferences,
            options: s_compilationOptions);

        IIncrementalGenerator generator = new MinimalApiBuilderGenerator();
        IEnumerable<ISourceGenerator> generators = GetSourceGenerators(generator);

        GeneratorDriver driver = CSharpGeneratorDriver
            .Create(generators, optionsProvider: optionsProvider, parseOptions: s_parseOptions)
            .RunGeneratorsAndUpdateCompilation(compilation, out var newCompilation, out _);

        AssertCompilation(newCompilation);

        await Task.WhenAll(Verify(driver).DisableDiff(), AssertCompilationWithAnalyzersAsync(newCompilation));
    }

    private static IEnumerable<ISourceGenerator> GetSourceGenerators(params IIncrementalGenerator[] generators)
    {
        return generators.Select(GeneratorExtensions.AsSourceGenerator);
    }

    private static void AssertCompilation(Compilation compilation)
    {
        MemoryStream output = new();
        EmitResult result = compilation.Emit(output);
        IEnumerable<Diagnostic> warningsOrWorse = WarningOrWorse(result.Diagnostics);
        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True);
            Assert.That(warningsOrWorse, Is.Empty);
        });
    }

    private static async Task AssertCompilationWithAnalyzersAsync(Compilation compilation)
    {
        compilation = RemoveAnnotationPreventingCodeAnalysis(compilation);
        CompilationWithAnalyzers withAnalyzers = compilation.WithAnalyzers(s_analyzers);
        ImmutableArray<Diagnostic> diagnostics = await withAnalyzers.GetAnalyzerDiagnosticsAsync();
        IEnumerable<Diagnostic> warningsOrWorse = WarningOrWorse(diagnostics);
        Assert.That(warningsOrWorse, Is.Empty);
    }

    private static Compilation RemoveAnnotationPreventingCodeAnalysis(Compilation compilation)
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

    private static IEnumerable<Diagnostic> WarningOrWorse(IEnumerable<Diagnostic> diagnostics)
    {
        return diagnostics.Where(static diagnostic => diagnostic.Severity >= DiagnosticSeverity.Warning);
    }
}
