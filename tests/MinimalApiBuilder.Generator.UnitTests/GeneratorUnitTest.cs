using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyInjection;

namespace MinimalApiBuilder.Generator.UnitTests;

public abstract class GeneratorUnitTest
{
    private static readonly CSharpParseOptions s_parseOptions = new(LanguageVersion.CSharp11);
    private static readonly CSharpCompilationOptions s_compilationOptions =
        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            .WithNullableContextOptions(NullableContextOptions.Enable);
    private static readonly string s_dllDirectory =
        Path.GetDirectoryName(typeof(object).Assembly.Location)
        ?? throw new InvalidOperationException("Cannot find object assembly directory");

    protected static Task VerifyGeneratorAsync(string source)
    {
        TestAnalyzerConfigOptionsProvider optionsProvider = new(
            globalOptions: new TestAnalyzerConfigOptions(),
            localOptions: new TestAnalyzerConfigOptions(),
            snapshotFolder: "default_configuration");

        return VerifyGeneratorAsync(source, optionsProvider);
    }

    protected static async Task VerifyGeneratorAsync(string source, TestAnalyzerConfigOptionsProvider optionsProvider)
    {
        MetadataReference[] references =
        {
            MetadataReference.CreateFromFile(typeof(RouteHandlerBuilder).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(MinimalApiBuilderEndpoint).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(AbstractValidator<>).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(ServiceLifetime).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Attribute).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(HttpContext).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(TypedResults).Assembly.Location),
            MetadataReference.CreateFromFile(Path.Join(s_dllDirectory, "System.Runtime.dll"))
        };

        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText($"""
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http.HttpResults;
using MinimalApiBuilder;
using FluentValidation;
using System.Threading.Tasks;
using System.Reflection;

{source}
""");

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: nameof(GeneratorUnitTest),
            syntaxTrees: new[] { syntaxTree },
            references: references,
            options: s_compilationOptions);

        IIncrementalGenerator generator = new MinimalApiBuilderGenerator();
        IEnumerable<ISourceGenerator> generators = GetSourceGenerators(generator);

        GeneratorDriver driver = CSharpGeneratorDriver
            .Create(generators, optionsProvider: optionsProvider, parseOptions: s_parseOptions)
            .RunGenerators(compilation);

        await Verify(driver).UseDirectory(optionsProvider.SnapshotFolder).DisableDiff();
    }

    private static IEnumerable<ISourceGenerator> GetSourceGenerators(params IIncrementalGenerator[] generators)
    {
        return generators.Select(GeneratorExtensions.AsSourceGenerator);
    }
}
