using MinimalApiBuilder.Generator.UnitTests.Infrastructure;

namespace MinimalApiBuilder.Generator.UnitTests;

internal sealed class AccessibilityTests : GeneratorUnitTest
{
    [Test]
    public Task Endpoint_Keeps_Accessibility_Level()
    {
        // language=cs
        const string source = """
            namespace MyNamespace;

            internal partial class E : MinimalApiBuilderEndpoint
            {
                public static int Handle(E e) => 0;
            }
            """;

        return VerifyGeneratorAsync(source);
    }
}
