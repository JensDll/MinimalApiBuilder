using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace MinimalApiBuilder.Generator.UnitTests;

public static class TestHelper
{
    public static async Task Verify(string source, TestAnalyzerConfigOptionsProvider provider)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

        MetadataReference[] references =
        {
            MetadataReference.CreateFromFile(typeof(RouteHandlerBuilder).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(MinimalApiBuilderEndpoint).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(AbstractValidator<>).Assembly.Location)
        };

        CSharpCompilationOptions options = new(OutputKind.DynamicallyLinkedLibrary);

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "MinimalApiBuilderGeneratorUnitTests",
            syntaxTrees: new[] { syntaxTree },
            references: references,
            options: options);

        IIncrementalGenerator generator = new MinimalApiBuilderGenerator();
        IEnumerable<ISourceGenerator> generators = GetSourceGenerators(generator);

        GeneratorDriver driver = CSharpGeneratorDriver
            .Create(generators, optionsProvider: provider)
            .RunGenerators(compilation);

        await Verifier.Verify(driver).UseDirectory(provider.SnapshotFolder).DisableDiff();
    }

    private static IEnumerable<ISourceGenerator> GetSourceGenerators(params IIncrementalGenerator[] generators)
    {
        return generators.Select(GeneratorExtensions.AsSourceGenerator);
    }
}
