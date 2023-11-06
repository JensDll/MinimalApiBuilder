namespace MinimalApiBuilder.Generator.UnitTests.Tests;

public class DiagnosticTests : GeneratorUnitTest
{
    [Test]
    public async Task Nullable_Value_Type_Validation()
    {
        // lang=cs
        const string source = """
public struct R
{ }

public partial class E : MinimalApiBuilderEndpoint
{
    public static int Handle(E e, R? r) => 0;
}

public class RValidator : AbstractValidator<R?>
{
    RValidator()
    { }
}
""";

        await VerifyGeneratorAsync(source);
    }
}
