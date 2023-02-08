using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace MinimalApiBuilder.Generator.IntegrationTest;

public static class TestHelper
{
    public static Task Verify(string source)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "MinimalApiBuilderGeneratorTests",
            syntaxTrees: new[] { syntaxTree });

        MinimalApiBuilderGenerator generator = new();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator).RunGenerators(compilation);

        return Verifier
            .Verify(driver)
            .UseDirectory("__snapshots__");
    }
}
