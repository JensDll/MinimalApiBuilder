namespace MinimalApiBuilder.Generator.UnitTests.Tests;

internal sealed class ConfigureEndpointsTests : GeneratorUnitTest
{
    [Test]
    public Task With_Using_Static()
    {
        const string source = """
            public partial class E : MinimalApiBuilderEndpoint
            {
                public static int Handle() => 10;
            }
            """;

        const string mapActions = """
            Configure(app.MapGet("/test", E.Handle));
            """;

        return VerifyGeneratorAsync(source, mapActions);
    }

    [Test]
    public Task With_Full_Name()
    {
        const string source = """
            public partial class E : MinimalApiBuilderEndpoint
            {
                public static int Handle() => 10;
            }
            """;

        const string mapActions = """
            ConfigureEndpoints.Configure(app.MapGet("/test", E.Handle));
            """;

        return VerifyGeneratorAsync(source, mapActions);
    }

    [Test]
    public Task With_Multiple_Endpoints()
    {
        const string source = """
            public partial class E1 : MinimalApiBuilderEndpoint
            {
                public static int Handle() => 0;
            }

            public partial class E2 : MinimalApiBuilderEndpoint
            {
                public static int Handle() => 0;
            }
            """;

        const string mapActions = """
            Configure(
                app.MapGet("/test1", E1.Handle),
                app.MapGet("/test2", E2.Handle));
            """;

        return VerifyGeneratorAsync(source, mapActions);
    }

    [Test]
    public Task With_Non_Endpoints_Between()
    {
        const string source = """
            public partial class E1 : MinimalApiBuilderEndpoint
            {
                public static int Handle() => 0;
            }

            public partial class E2 : MinimalApiBuilderEndpoint
            {
                public static int Handle() => 0;
            }
            """;

        const string mapActions = """
            Configure(
                app.MapGet("/foo", static () => 0),
                app.MapGet("/test1", E1.Handle),
                app.MapGet("/test2", E2.Handle),
                app.MapGet("/bar", static () => 0));
            """;

        return VerifyGeneratorAsync(source, mapActions);
    }

    [Test]
    public Task With_Overlapping_Arity()
    {
        const string source = """
            public partial class E1 : MinimalApiBuilderEndpoint
            {
                public static int Handle() => 0;
            }

            public partial class E2 : MinimalApiBuilderEndpoint
            {
                public static int Handle() => 0;
            }
            """;

        const string mapActions = """
            Configure(
                app.MapGet("/test1", E1.Handle),
                app.MapGet("/test2", E2.Handle));

            Configure(
                app.MapGet("/test3", E1.Handle),
                app.MapGet("/test4", E2.Handle));
            """;

        return VerifyGeneratorAsync(source, mapActions);
    }

    [Test]
    public Task With_Overlapping_Arity_And_Non_Endpoints_Between()
    {
        const string source = """
            public partial class E1 : MinimalApiBuilderEndpoint
            {
                public static int Handle() => 0;
            }

            public partial class E2 : MinimalApiBuilderEndpoint
            {
                public static int Handle() => 0;
            }
            """;

        const string mapActions = """
            Configure(
                app.MapGet("/test1", E1.Handle),
                app.MapGet("/foo", static () => 0),
                app.MapGet("/test2", E2.Handle));

            Configure(
                app.MapGet("/bar", static () => 0),
                app.MapGet("/test3", E1.Handle),
                app.MapGet("/test4", E2.Handle));
            """;

        return VerifyGeneratorAsync(source, mapActions);
    }

    [Test]
    public Task With_Multiple_Overlapping_Arity()
    {
        const string source = """
            public partial class E1 : MinimalApiBuilderEndpoint
            {
                public static int Handle() => 0;
            }

            public partial class E2 : MinimalApiBuilderEndpoint
            {
                public static int Handle() => 0;
            }

            public partial class E3 : MinimalApiBuilderEndpoint
            {
                public static int Handle() => 0;
            }
            
            public partial class E4 : MinimalApiBuilderEndpoint
            {
                public static int Handle() => 0;
            }
            """;

        const string mapActions = """
            Configure(
                app.MapGet("/test1", E1.Handle),
                app.MapGet("/test2", E2.Handle));

            Configure(
                app.MapGet("/test3", E1.Handle),
                app.MapGet("/test4", E2.Handle));

            Configure(
                app.MapGet("/test5", E3.Handle));
            
            Configure(
                app.MapGet("/test6", E4.Handle));
            """;

        return VerifyGeneratorAsync(source, mapActions);
    }

    [Test]
    public Task With_Overlapping_And_Non_Overlapping_Arity()
    {
        const string source = """
            public partial class E : MinimalApiBuilderEndpoint
            {
                public static int Handle() => 0;
            }
            """;

        const string mapActions = """
            Configure(
                app.MapGet("/test1", E.Handle));

            Configure(
                app.MapGet("/test2", E.Handle));

            Configure(
                app.MapGet("/test3", E.Handle),
                app.MapGet("/test4", E.Handle));
            """;

        return VerifyGeneratorAsync(source, mapActions);
    }
}
