using MinimalApiBuilder.Generator.UnitTests.Infrastructure;

namespace MinimalApiBuilder.Generator.UnitTests;

internal sealed class CustomBindingTests : GeneratorUnitTest
{
    [Test]
    public Task BindAsync([Values] bool nullable, [ValueSource(nameof(Validation))] (string, string) validation)
    {
        string nullableMark = GetNullableMark(nullable);
        string rule1 = GetRule(validation.Item1);
        string rule2 = GetRule(validation.Item2);

        // language=cs
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
                public static int Handle(E1 e, R1 r1) => 1;
            }

            public partial class E2 : MinimalApiBuilderEndpoint
            {
                public static int Handle(E2 e, R1? r1) => 1;
            }

            public partial class E3 : MinimalApiBuilderEndpoint
            {
                public static int Handle(E3 e, R1 r1, R2 r2) => 1;
            }

            public partial class E4 : MinimalApiBuilderEndpoint
            {
                public static int Handle(E4 e, R1? r1, R2 r2) => 1;
            }

            public partial class E5 : MinimalApiBuilderEndpoint
            {
                public static int Handle(E5 e, R1? r1, R2? r2) => 1;
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

        return VerifyGenerator(source);
    }

    [Test]
    public Task TryParse([Values] bool nullable, [ValueSource(nameof(Validation))] (string, string) validation)
    {
        string nullableMark = GetNullableMark(nullable);
        string rule1 = GetRule(validation.Item1);
        string rule2 = GetRule(validation.Item2);

        // language=cs
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
                public static int Handle(E1 e, R1 r1) => 1;
            }

            public partial class E2 : MinimalApiBuilderEndpoint
            {
                public static int Handle(E2 e, R1? r1) => 1;
            }

            public partial class E3 : MinimalApiBuilderEndpoint
            {
                public static int Handle(E3 e, R1 r1, R2 r2) => 1;
            }

            public partial class E4 : MinimalApiBuilderEndpoint
            {
                public static int Handle(E4 e, R1? r1, R2 r2) => 1;
            }

            public partial class E5 : MinimalApiBuilderEndpoint
            {
                public static int Handle(E5 e, R1? r1, R2? r2) => 1;
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

        return VerifyGenerator(source);
    }

    [Test]
    public Task BindAsync_Without_Validators([Values] bool nullable)
    {
        string nullableMark = GetNullableMark(nullable);

        // language=cs
        string source = $$"""
            public class R {
                public int Value { get; set; }
                public static ValueTask<R{{nullableMark}}> BindAsync(HttpContext context)
                {
                    return ValueTask.FromResult<R{{nullableMark}}>(new R());
                }
            }

            public partial class E : MinimalApiBuilderEndpoint
            {
                public static int Handle(E e, R r) => 1;
            }
            """;

        return VerifyGenerator(source);
    }

    [Test]
    public Task TryParse_Without_Validators([Values] bool nullable)
    {
        string nullableMark = GetNullableMark(nullable);

        // language=cs
        string source = $$"""
            public class R {
                public int Value { get; set; }
                public static bool TryParse(string value, out R{{nullableMark}} r)
                {
                    r = new R();
                    return false;
                }
            }

            public partial class E : MinimalApiBuilderEndpoint
            {
                public static int Handle(E e, R r) => 1;
            }
            """;

        return VerifyGenerator(source);
    }

    private static IEnumerable<(string, string)> Validation()
    {
        yield return ("Must", "Must");
        yield return ("MustAsync", "Must");
        yield return ("Must", "MustAsync");
        yield return ("MustAsync", "MustAsync");
    }

    private static string GetNullableMark(bool nullable) => nullable ? "?" : "";

    private static string GetRule(string validation) => validation == "Must"
        ? "RuleFor(static x => x.Value).Must(static _ => true)"
        : "RuleFor(static x => x.Value).MustAsync(static (_, _) => Task.FromResult(true))";
}
