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

        string rule1 = validation.Item1 == "Must"
            ? "RuleFor(static x => x.Value).Must(static _ => true)"
            : "RuleFor(static x => x.Value).MustAsync(static (_, _) => Task.FromResult(true))";

        string rule2 = validation.Item2 == "Must"
            ? "RuleFor(static x => x.Value).Must(static _ => true)"
            : "RuleFor(static x => x.Value).MustAsync(static (_, _) => Task.FromResult(true))";

        // lang=cs
        string source = $$"""
public class R1 {
    public int Value { get; set; }
    public static ValueTask<R1{{nullableMark}}> BindAsync(HttpContext context)
    {
        return ValueTask.FromResult<R1{{nullableMark}}>(new R1());
    }
}

public class R2 {
    public int Value { get; set; }
    public static ValueTask<R2{{nullableMark}}> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        return ValueTask.FromResult<R2{{nullableMark}}>(new R2());
    }
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
        {{rule1}};
    }
}

public class R2Validator : AbstractValidator<R2>
{
    public R2Validator()
    {
        {{rule2}};
    }
}
""";

        return VerifyGeneratorAsync(source);
    }

    [Test]
    public async Task TryParse([Values] bool nullable, [ValueSource(nameof(Validation))] (string, string) validation)
    {
        string nullableMark = nullable ? "?" : "";

        string rule1 = validation.Item1 == "Must"
            ? "RuleFor(static x => x.Value).Must(static _ => true)"
            : "RuleFor(static x => x.Value).MustAsync(static (_, _) => Task.FromResult(true))";

        string rule2 = validation.Item2 == "Must"
            ? "RuleFor(static x => x.Value).Must(static _ => true)"
            : "RuleFor(static x => x.Value).MustAsync(static (_, _) => Task.FromResult(true))";

        // lang=cs
        string source = $$"""
public class R1 {
    public int Value { get; set; }
    public static bool TryParse(string value, out R1{{nullableMark}} r1)
    {
        r1 = new R1();
        return false;
    }
}

public class R2 {
    public int Value { get; set; }
    public static bool TryParse(string value, IFormatProvider format, out R2{{nullableMark}} r2)
    {
        r2 = new R2();
        return false;
    }
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
        {{rule1}};
    }
}

public class R2Validator : AbstractValidator<R2>
{
    public R2Validator()
    {
        {{rule2}};
    }
}
""";

        await VerifyGeneratorAsync(source);
    }
}
