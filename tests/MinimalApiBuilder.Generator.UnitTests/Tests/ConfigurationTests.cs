using MinimalApiBuilder.Generator.Entities;

namespace MinimalApiBuilder.Generator.UnitTests.Tests;

internal sealed class ConfigurationTests : GeneratorUnitTest
{
    [Test]
    public Task Assign_Name(
        [ValueSource(nameof(AssignNameProviders))]
        TestAnalyzerConfigOptionsProvider provider)
    {
        // language=cs
        const string source = """
            public partial class E : MinimalApiBuilderEndpoint
            {
                public static int Handle(E e) => 0;
            }
            """;

        return VerifyGeneratorAsync(source, provider);
    }

    [Test]
    public Task Validation_Problem_Title(
        [ValueSource(nameof(ValidationProblemTitleProviders))]
        TestAnalyzerConfigOptionsProvider provider)
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
                public static int Handle(R r) => r.Value;
            }

            public class RValidator : AbstractValidator<R>
            {
                public RValidator()
                {
                    RuleFor(static x => x.Value).GreaterThan(0);
                }
            }
            """;

        return VerifyGeneratorAsync(source, provider);
    }

    [Test]
    public Task With_Empty_Options_Should_Keep_Default_Values()
    {
        TestAnalyzerConfigOptionsProvider provider = new(
            globalOptions: new TestAnalyzerConfigOptions
            {
                Options =
                {
                    [GeneratorOptions.Keys.AssignNameToEndpointBuildProperty] = "",
                    [GeneratorOptions.Keys.ValidationProblemTitleBuildProperty] = "",
                    [GeneratorOptions.Keys.ModelBindingProblemTitleBuildProperty] = ""
                }
            },
            localOptions: new TestAnalyzerConfigOptions
            {
                Options =
                {
                    [GeneratorOptions.Keys.AssignNameToEndpoint] = "",
                    [GeneratorOptions.Keys.ValidationProblemTitle] = "",
                    [GeneratorOptions.Keys.ModelBindingProblemTitle] = ""
                }
            });

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
                public static int Handle(R r) => r.Value;
            }

            public class RValidator : AbstractValidator<R>
            {
                public RValidator()
                {
                    RuleFor(static x => x.Value).GreaterThan(0);
                }
            }
            """;

        return VerifyGeneratorAsync(source, provider);
    }

    private static IEnumerable<TestAnalyzerConfigOptionsProvider> AssignNameProviders()
    {
        yield return new TestAnalyzerConfigOptionsProvider(
            globalOptions: new TestAnalyzerConfigOptions
            {
                Options =
                {
                    [GeneratorOptions.Keys.AssignNameToEndpointBuildProperty] = "true"
                }
            },
            localOptions: new TestAnalyzerConfigOptions(),
            friendlyName: "assign_name_global_true");

        yield return new TestAnalyzerConfigOptionsProvider(
            globalOptions: new TestAnalyzerConfigOptions(),
            localOptions: new TestAnalyzerConfigOptions
            {
                Options =
                {
                    [GeneratorOptions.Keys.AssignNameToEndpoint] = "true"
                }
            },
            friendlyName: "assign_name_local_true");

        yield return new TestAnalyzerConfigOptionsProvider(
            globalOptions: new TestAnalyzerConfigOptions
            {
                Options =
                {
                    [GeneratorOptions.Keys.AssignNameToEndpointBuildProperty] = "true"
                }
            },
            localOptions: new TestAnalyzerConfigOptions
            {
                Options =
                {
                    [GeneratorOptions.Keys.AssignNameToEndpoint] = "false"
                }
            },
            friendlyName: "assign_name_global_true_local_false");
    }

    private static IEnumerable<TestAnalyzerConfigOptionsProvider> ValidationProblemTitleProviders()
    {
        yield return new TestAnalyzerConfigOptionsProvider(
            globalOptions: new TestAnalyzerConfigOptions
            {
                Options =
                {
                    [GeneratorOptions.Keys.ValidationProblemTitleBuildProperty] = "validation_problem_title_global"
                }
            },
            localOptions: new TestAnalyzerConfigOptions(),
            friendlyName: "validation_problem_title_global");

        yield return new TestAnalyzerConfigOptionsProvider(
            globalOptions: new TestAnalyzerConfigOptions(),
            localOptions: new TestAnalyzerConfigOptions
            {
                Options =
                {
                    [GeneratorOptions.Keys.ValidationProblemTitle] = "validation_problem_title_local"
                }
            },
            friendlyName: "validation_problem_title_local");

        yield return new TestAnalyzerConfigOptionsProvider(
            globalOptions: new TestAnalyzerConfigOptions
            {
                Options =
                {
                    [GeneratorOptions.Keys.ModelBindingProblemTitleBuildProperty] =
                        "model_binding_problem_title_global"
                }
            },
            localOptions: new TestAnalyzerConfigOptions(),
            friendlyName: "model_binding_problem_title_global");

        yield return new TestAnalyzerConfigOptionsProvider(
            globalOptions: new TestAnalyzerConfigOptions(),
            localOptions: new TestAnalyzerConfigOptions
            {
                Options =
                {
                    [GeneratorOptions.Keys.ModelBindingProblemTitle] = "model_binding_problem_title_local"
                }
            },
            friendlyName: "model_binding_problem_title_local");
    }
}
