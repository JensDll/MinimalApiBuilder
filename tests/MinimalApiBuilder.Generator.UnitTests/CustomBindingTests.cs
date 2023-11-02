namespace MinimalApiBuilder.Generator.UnitTests;

[TestFixture]
public class CustomBindingTests
{
    private static IEnumerable<(string, string)> ValidatorCombinations()
    {
        yield return ("Must", "Must");
        yield return ("MustAsync", "Must");
        yield return ("Must", "MustAsync");
        yield return ("MustAsync", "MustAsync");
    }

    [Test]
    public Task BindAsync([Values("?", "")] string nullable,
        [ValueSource(nameof(ValidatorCombinations))]
        (string, string) validatorCombinations)
    {
        // lang=cs
        string source = $$"""
using MinimalApiBuilder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Reflection;
using FluentValidation;

public class R1 {
    int Value { get; set; }
    public static async ValueTask<R1{{nullable}}> BindAsync(HttpContext context)
    {
        await Task.CompletedTask;
        return null;
    }
}

public struct R2 {
    int Value { get; set; }
    public static async ValueTask<R2{{nullable}}> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        await Task.CompletedTask;
        return null;
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
        RuleFor(static x => x.Value).{{validatorCombinations.Item1}}();
    }
}

public class R2Validator : AbstractValidator<R2>
{
    public R2Validator()
    {
        RuleFor(static x => x.Value).{{validatorCombinations.Item2}}();
    }
}
""";

        return TestHelper.Verify(source);
    }

    [Test]
    public Task TryParse([Values("?", "")] string nullable,
        [ValueSource(nameof(ValidatorCombinations))]
        (string, string) validatorCombinations)
    {
        // lang=cs
        string source = $$"""
using System;
using MinimalApiBuilder;
using FluentValidation;

public class R1 {
    int Value { get; set; }
    public static bool TryParse(string value, out R1{{nullable}} r1)
    {
        r1 = null;
        return false;
    }
}

public struct R2 {
    int Value { get; set; }
    public static bool TryParse(string value, IFormatProvider format, out R2{{nullable}} r2)
    {
        r1 = null;
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
        RuleFor(static x => x.Value).{{validatorCombinations.Item1}}();
    }
}

public class R2Validator : AbstractValidator<R2>
{
    public R2Validator()
    {
        RuleFor(static x => x.Value).{{validatorCombinations.Item2}}();
    }
}
""";

        return TestHelper.Verify(source);
    }

    [Test]
    public Task Foo()
    {
        // lang=cs
        const string source = """
using MinimalApiBuilder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Reflection;
using FluentValidation;

public struct R {
    public static async ValueTask<R?> BindAsync(HttpContext context, ParameterInfo info)
    {
        await Task.CompletedTask;
        return null;
    }
}

public partial class E : MinimalApiBuilderEndpoint
{
    private static int Handle(E e, R? r) => 1;
}
""";

        return TestHelper.Verify(source);
    }
}
