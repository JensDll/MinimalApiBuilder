namespace MinimalApiBuilder.Generator.UnitTests;

[UsesVerify]
public class CombinedValidationTests
{
    [Theory]
    [ClassData(typeof(TestAnalyzerConfigOptionsProviderClassData))]
    public Task With_Two_Requests(TestAnalyzerConfigOptionsProvider provider)
    {
        // lang=cs
        const string source = """
            using MinimalApiBuilder;
            using FluentValidation;

            namespace Features;

            public partial class Endpoint : MinimalApiBuilderEndpoint
            {
                private static IResult Handle(Endpoint e, FirstRequest r1, SecondRequest r2) => 1;
            }

            public class FirstRequest
            {
                public string Value { get; set; }
            }

            public class SyncValidator : AbstractValidator<FirstRequest>
            {
                public SyncValidator()
                {
                    RuleFor(x => x.Value).NotEmpty();
                }
            }

            public class SecondRequest
            {
                public string Value { get; set; }
            }

            public class AsyncValidator : AbstractValidator<SecondRequest>
            {
                public AsyncValidator()
                {
                    RuleFor(x => x.Value).MustAsync((value, _) => Task.FromResult(true));
                }
            }
            """;

        return TestHelper.Verify(source, provider);
    }
}
