namespace MinimalApiBuilder.Generator.UnitTests.Tests;

public class CustomBindingTests : GeneratorUnitTest
{
    private static IEnumerable<(string, string)> Validation()
    {
        yield return ("Must", "Must");
        yield return ("MustAsync", "Must");
        yield return ("Must", "MustAsync");
        yield return ("MustAsync", "MustAsync");
    }

    [Test]
    public Task BindAsync([Values] bool nullable, [ValueSource(nameof(Validation))] (string, string) validation)
    {
        string nullableMark = nullable ? "?" : "";

        // lang=cs
        string source = $$"""
public class R1 {
    int Value { get; set; }
    public static async ValueTask<R1{{nullableMark}}> BindAsync(HttpContext context)
    { }
}

public class R2 {
    int Value { get; set; }
    public static async ValueTask<R2{{nullableMark}}> BindAsync(HttpContext context, ParameterInfo parameter)
    { }
}

public partial class E1 : MinimalApiBuilderEndpoint
{
    private static int Handle(E1 e, R1 r1) => 1;
}

public partial class E2 : MinimalApiBuilderEndpoint
{
    private static int Handle(E2 e, R1? r1) => 1;
}

public partial class E3 : MinimalApiBuilderEndpoint
{
    private static int Handle(E3 e, R1 r1, R2 r2) => 1;
}

public partial class E4 : MinimalApiBuilderEndpoint
{
    private static int Handle(E4 e, R1? r1, R2 r2) => 1;
}

public partial class E5 : MinimalApiBuilderEndpoint
{
    private static int Handle(E5 e, R1? r1, R2? r2) => 1;
}

public class R1Validator : AbstractValidator<R1>
{
    public R1Validator()
    {
        RuleFor(static x => x.Value).{{validation.Item1}}();
    }
}

public class R2Validator : AbstractValidator<R2>
{
    public R2Validator()
    {
        RuleFor(static x => x.Value).{{validation.Item2}}();
    }
}
""";

        return VerifyGeneratorAsync(source);
    }

    [Test]
    public async Task TryParse([Values] bool nullable, [ValueSource(nameof(Validation))] (string, string) validation)
    {
        string nullableMark = nullable ? "?" : "";

        // lang=cs
        string source = $$"""
public class R1 {
    int Value { get; set; }
    public static bool TryParse(string value, out R1{{nullableMark}} r1)
    { }
}

public class R2 {
    int Value { get; set; }
    public static bool TryParse(string value, IFormatProvider format, out R2{{nullableMark}} r2)
    { }
}

public partial class E1 : MinimalApiBuilderEndpoint
{
    private static int Handle(E1 e, R1 r1) => 1;
}

public partial class E2 : MinimalApiBuilderEndpoint
{
    private static int Handle(E2 e, R1? r1) => 1;
}

public partial class E3 : MinimalApiBuilderEndpoint
{
    private static int Handle(E3 e, R1 r1, R2 r2) => 1;
}

public partial class E4 : MinimalApiBuilderEndpoint
{
    private static int Handle(E4 e, R1? r1, R2 r2) => 1;
}

public partial class E5 : MinimalApiBuilderEndpoint
{
    private static int Handle(E5 e, R1? r1, R2? r2) => 1;
}

public class R1Validator : AbstractValidator<R1>
{
    public R1Validator()
    {
        RuleFor(static x => x.Value).{{validation.Item1}}();
    }
}

public class R2Validator : AbstractValidator<R2>
{
    public R2Validator()
    {
        RuleFor(static x => x.Value).{{validation.Item2}}();
    }
}
""";

        await VerifyGeneratorAsync(source);
    }
}
