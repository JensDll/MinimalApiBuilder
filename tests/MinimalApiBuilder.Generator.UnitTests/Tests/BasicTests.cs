namespace MinimalApiBuilder.Generator.UnitTests.Tests;

internal sealed class BasicTests : GeneratorUnitTest
{
    [Test]
    public Task With_Configure()
    {
        // language=cs
        const string source = """
            public partial class E : MinimalApiBuilderEndpoint
            {
                public static int Handle(E e) => 0;

                public static void Configure(RouteHandlerBuilder builder)
                { }
            }
            """;

        return VerifyGeneratorAsync(source);
    }

    [Test]
    public Task Without_Configure()
    {
        // language=cs
        const string source = """
            public partial class E : MinimalApiBuilderEndpoint
            {
                public static int Handle(E e) => 0;
            }
            """;

        return VerifyGeneratorAsync(source);
    }

    [Test]
    public Task In_Namespace()
    {
        // language=cs
        const string source = """
            namespace MyNamespace;

            public partial class E : MinimalApiBuilderEndpoint
            {
                public static int Handle(E e) => 0;
            }
            """;

        return VerifyGeneratorAsync(source);
    }

    [Test]
    public Task Endpoint_Parameter_At_Different_Locations([Values(0, 1, 2, 3)] int location)
    {
        List<string> parameters = ["int a", "R r", "int b"];
        parameters.Insert(location, "E e");
        string parametersString = string.Join(", ", parameters);

        // language=cs
        string source = $$"""
            public class R {
                public int Value { get; set; }
                public static ValueTask<R> BindAsync(HttpContext context)
                {
                    return ValueTask.FromResult<R>(new R());
                }
            }

            public partial class E : MinimalApiBuilderEndpoint
            {
                public static int Handle({{parametersString}}) => 0;
            }

            public class RValidator : AbstractValidator<R>
            {
                public RValidator()
                {
                    RuleFor(static x => x.Value).GreaterThan(0);
                }
            }
            """;

        return VerifyGeneratorAsync(source);
    }

    [Test]
    public Task Without_Endpoint_Parameter()
    {
        // language=cs
        const string source = """
            public class R {
                public int Value { get; set; }
            }

            public partial class E : MinimalApiBuilderEndpoint
            {
                public static int Handle(int a, int b, R r) => a;
            }

            public class RValidator : AbstractValidator<R>
            {
                public RValidator()
                {
                    RuleFor(static x => x.Value).GreaterThan(0);
                }
            }
            """;

        // language=cs
        const string mapActions = """
            app.MapDelete("/test/{a:int}/{b:int}", E.Handle);
            """;

        return VerifyGeneratorAsync(source, mapActions);
    }

    [Test]
    public Task Without_Endpoint_Parameter_And_Custom_Binding()
    {
        // language=cs
        const string source = """
            public class R {
                public int Value { get; set; }
                public static ValueTask<R> BindAsync(HttpContext context)
                {
                    return ValueTask.FromResult<R>(new R());
                }
            }

            public partial class E : MinimalApiBuilderEndpoint
            {
                public static int Handle(int a, int b, R r) => a;
            }

            public class RValidator : AbstractValidator<R>
            {
                public RValidator()
                {
                    RuleFor(static x => x.Value).GreaterThan(0);
                }
            }
            """;

        return VerifyGeneratorAsync(source);
    }
}
