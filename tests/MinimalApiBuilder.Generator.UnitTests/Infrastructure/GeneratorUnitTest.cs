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

namespace MinimalApiBuilder.Generator.UnitTests.Infrastructure;

internal abstract class GeneratorUnitTest
{
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
        Directory.GetFiles("copied-analyzers", "Microsoft.CodeAnalysis.*.dll")
            .Where(static path => !path.EndsWith("Fixes.dll", StringComparison.Ordinal))
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
            .ChangeAndReturn("CA1050", ReportDiagnostic.Suppress) // Declare types in namespaces
            .ChangeAndReturn("CA1062", ReportDiagnostic.Suppress) // Validate arguments of public methods
            .ChangeAndReturn("CA1707", ReportDiagnostic.Suppress) // Identifiers should not contain underscores
            .ChangeAndReturn("CA1812", ReportDiagnostic.Suppress) // Avoid uninstantiated internal classes
            .ChangeAndReturn("CA1849", ReportDiagnostic.Suppress) // Call async methods when in an async method
            .ChangeAndReturn("CA1852", ReportDiagnostic.Suppress) // Seal internal types
            .ChangeAndReturn("CA2007", ReportDiagnostic.Suppress); // Do not directly await a Task

    private static readonly CSharpCompilationOptions s_compilationOptions =
        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            .WithNullableContextOptions(NullableContextOptions.Enable)
            .WithSpecificDiagnosticOptions(s_diagnosticsOptions);

    private static readonly CSharpParseOptions s_parseOptions = new(LanguageVersion.CSharp11);

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
        AnalyzerConfigOptionsProvider optionsProvider)
    {
        return VerifyGeneratorAsyncImpl(GetSource(source), optionsProvider);
    }

    private static Task VerifyGeneratorAsync(
        string source,
        string mapActions,
        AnalyzerConfigOptionsProvider optionsProvider)
    {
        return VerifyGeneratorAsyncImpl(GetSource(source, mapActions), optionsProvider);
    }

    private static Task VerifyGeneratorAsyncImpl(string source, AnalyzerConfigOptionsProvider optionsProvider)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source, options: s_parseOptions);

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

        return Task.WhenAll(
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
        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True);
            Assert.That(result.Diagnostics.WarningsOrWorse(), Is.Empty);
        });
    }

    private static async Task AssertCompilationWithAnalyzersAsync(Compilation compilation)
    {
        compilation = RemoveAnnotationsPreventingCodeAnalysis(compilation);
        CompilationWithAnalyzers withAnalyzers = compilation.WithAnalyzers(s_analyzers);
        ImmutableArray<Diagnostic> diagnostics = await withAnalyzers.GetAnalyzerDiagnosticsAsync();
        Assert.That(diagnostics.WarningsOrWorse(), Is.Empty);
    }

    private static Compilation RemoveAnnotationsPreventingCodeAnalysis(Compilation compilation)
    {
        GeneratedCodeRewriter rewriter = new();

        foreach (SyntaxTree syntaxTree in compilation.SyntaxTrees)
        {
            if (!syntaxTree.FilePath.EndsWith(".g.cs", StringComparison.Ordinal))
            {
                continue;
            }

            CompilationUnitSyntax root = syntaxTree.GetCompilationUnitRoot().WithoutLeadingTrivia();
            SyntaxNode newRoot = rewriter.Visit(root);
            SyntaxTree newSyntaxTree = newRoot.SyntaxTree.WithRootAndOptions(newRoot, s_parseOptions);

            compilation = compilation.ReplaceSyntaxTree(syntaxTree, newSyntaxTree);
        }

        return compilation;
    }

    private static string GetSource(string source)
    {
        // language=cs
        return $"""
            using Microsoft.AspNetCore.Builder;
            using Microsoft.AspNetCore.Http;
            using Microsoft.AspNetCore.Http.HttpResults;
            using Microsoft.AspNetCore.Mvc;
            using Microsoft.AspNetCore.Routing;
            using Microsoft.Extensions.DependencyInjection;
            using MinimalApiBuilder;
            using MinimalApiBuilder.Generator;
            using static MinimalApiBuilder.Generator.ConfigureEndpoints;
            using FluentValidation;
            using System;
            using System.Reflection;
            using System.Runtime.InteropServices;
            using System.Threading.Tasks;

            [assembly: AssemblyVersion("1.0")]
            [assembly: CLSCompliant(false)]
            [assembly: ComVisible(false)]

            {source}
            """;
    }

    private static string GetSource(string source, string mapActions)
    {
        // language=cs
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
