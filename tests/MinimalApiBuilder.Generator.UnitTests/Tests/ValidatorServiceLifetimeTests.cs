using Microsoft.Extensions.DependencyInjection;

namespace MinimalApiBuilder.Generator.UnitTests.Tests;

public class ValidatorServiceLifetimeTests : GeneratorUnitTest
{
    [Test]
    public Task With_Single_Attribute(
        [Values(nameof(ServiceLifetime.Singleton),
            nameof(ServiceLifetime.Scoped),
            nameof(ServiceLifetime.Transient))]
        string lifetime)
    {
        // lang=cs
        string source = $$"""
public class R {
    public int Value { get; set; }
}

public partial class E : MinimalApiBuilderEndpoint
{
    public static int Handle(E e) => 0;
}

[RegisterValidator(ServiceLifetime.{{lifetime}})]
public class RValidator : AbstractValidator<R>
{
    public RValidator()
    { }
}
""";
        return VerifyGeneratorAsync(source);
    }

    [Test]
    public Task With_Multiple_Attributes(
        [Values(nameof(ServiceLifetime.Singleton),
            nameof(ServiceLifetime.Scoped),
            nameof(ServiceLifetime.Transient))]
        string lifetime)
    {
        // lang=cs
        string source = $$"""
public class R {
    public int Value { get; set; }
}

public partial class E : MinimalApiBuilderEndpoint
{
    public static int Handle(E e) => 0;
}

[Route("Some Attribute")]
[Route("Some Other Attribute"), RegisterValidator(ServiceLifetime.{{lifetime}})]
public class RValidator : AbstractValidator<R>
{
    public RValidator()
    { }
}
""";
        return VerifyGeneratorAsync(source);
    }
}
