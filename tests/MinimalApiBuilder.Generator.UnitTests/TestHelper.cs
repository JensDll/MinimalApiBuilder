using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyInjection;
using MinimalApiBuilder.Generator.UnitTests.Fixtures;

namespace MinimalApiBuilder.Generator.UnitTests;

public static class TestHelper
{
    public static SettingsTask Verify(string source)
    {
        TestAnalyzerConfigOptionsProvider optionsProvider = new(
            globalOptions: new TestAnalyzerConfigOptions(),
            localOptions: new TestAnalyzerConfigOptions(),
            snapshotFolder: "default");

        return Verify(source, optionsProvider);
    }

    public static SettingsTask Verify(string source, TestAnalyzerConfigOptionsProvider optionsProvider)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

        string dllDirectory = Path.GetDirectoryName(typeof(object).Assembly.Location)
                              ?? throw new InvalidOperationException("Cannot find object assembly directory");

        MetadataReference[] references =
        {
            MetadataReference.CreateFromFile(typeof(RouteHandlerBuilder).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(MinimalApiBuilderEndpoint).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(AbstractValidator<>).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(ServiceLifetime).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Attribute).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(HttpContext).Assembly.Location),
            MetadataReference.CreateFromFile(Path.Join(dllDirectory, "System.Runtime.dll"))
        };

        CSharpCompilationOptions options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            .WithNullableContextOptions(NullableContextOptions.Enable);

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "MinimalApiBuilderGeneratorUnitTests",
            syntaxTrees: new[] { syntaxTree },
            references: references,
            options: options);

        IIncrementalGenerator generator = new MinimalApiBuilderGenerator();
        IEnumerable<ISourceGenerator> generators = GetSourceGenerators(generator);

        GeneratorDriver driver = CSharpGeneratorDriver
            .Create(generators, optionsProvider: optionsProvider)
            .RunGenerators(compilation);

        return Verifier.Verify(driver).UseDirectory(optionsProvider.SnapshotFolder).DisableDiff();
    }

    private static IEnumerable<ISourceGenerator> GetSourceGenerators(params IIncrementalGenerator[] generators)
    {
        return generators.Select(GeneratorExtensions.AsSourceGenerator);
    }
}
