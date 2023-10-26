namespace MinimalApiBuilder.Generator.UnitTests;

[UsesVerify]
public class SyncValidationTests
{
    [Theory]
    [ClassData(typeof(TestAnalyzerConfigOptionsProviderClassData))]
    public Task With_One_Request(TestAnalyzerConfigOptionsProvider provider)
    {
        // lang=cs
        const string source = """
            using FluentValidation;
            using MinimalApiBuilder;

            namespace Features;

            public partial class Endpoint : MinimalApiBuilderEndpoint
            {
                private static int Handle(Endpoint e, Request r) => 1;
            }

            public class Request
            {
                public string Value { get; set; }
            }

            public class Validator : AbstractValidator<Request>
            {
                public Validator()
                {
                    RuleFor(x => x.Value).NotEmpty();
                }
            }
            """;

        return TestHelper.Verify(source, provider);
    }

    [Theory]
    [ClassData(typeof(TestAnalyzerConfigOptionsProviderClassData))]
    public Task With_Two_Requests(TestAnalyzerConfigOptionsProvider provider)
    {
        // lang=cs
        const string source = """
            using FluentValidation;
            using MinimalApiBuilder;

            namespace Features;

            public partial class Endpoint : MinimalApiBuilderEndpoint
            {
                private static int Handle(Endpoint e, Request r1, Request r2) => 1;
            }

            public class Request
            {
                public string Value { get; set; }
            }

            public class Validator : AbstractValidator<Request>
            {
                public Validator()
                {
                    RuleFor(x => x.Value).NotEmpty();
                }
            }
            """;

        return TestHelper.Verify(source, provider);
    }
}
