namespace MinimalApiBuilder.Generator.UnitTests.Tests;

internal sealed class AccessibilityTests : GeneratorUnitTest
{
    [Test]
    public Task Endpoint_Keeps_Accessibility_Level()
    {
        // lang=cs
        const string source = """
namespace MyNamespace;

internal partial class E : MinimalApiBuilderEndpoint
{
    public static int Handle(E e) => 0;
}
""";

        // lang=cs
        const string mapActions = """
app.MapGet("/test", E.Handle);
""";

        return VerifyGeneratorAsync(source, mapActions);
    }
}
