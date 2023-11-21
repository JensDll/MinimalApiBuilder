namespace MinimalApiBuilder.Generator.UnitTests.Tests;

internal sealed class DiagnosticTests : GeneratorUnitTest
{
    [Test]
    public Task Nullable_Value_Type_Validation()
    {
        // language=cs
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

        return VerifyGeneratorAsync(source);
    }
}
