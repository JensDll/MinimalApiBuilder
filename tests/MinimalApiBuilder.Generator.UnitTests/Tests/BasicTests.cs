namespace MinimalApiBuilder.Generator.UnitTests.Tests;

internal sealed class BasicTests : GeneratorUnitTest
{
    [Test]
    public Task With_Configure()
    {
        // lang=cs
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
        // lang=cs
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
        // lang=cs
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
        List<string> parameters = new()
        {
            "int a",
            "R r",
            "int b"
        };
        parameters.Insert(location, "E e");

        string parametersString = string.Join(", ", parameters);

        // lang=cs
        string source = $$"""
public class R {
    public int Value { get; set; }
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
        // lang=cs
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

        // lang=cs
        const string mapActions = """
app.MapDelete<E>("/test/{a:int}/{b:int}");
""";

        return VerifyGeneratorAsync(source, mapActions);
    }
}
