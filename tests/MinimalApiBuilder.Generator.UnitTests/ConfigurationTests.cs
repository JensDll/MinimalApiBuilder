using MinimalApiBuilder.Generator.Entities;
using MinimalApiBuilder.Generator.UnitTests.Infrastructure;

namespace MinimalApiBuilder.Generator.UnitTests;

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
    public Task Validation_Problem(
        [ValueSource(nameof(ValidationProblemProviders))]
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
                    [GeneratorOptions.Keys.ValidationProblemTypeBuildProperty] = "",
                    [GeneratorOptions.Keys.ValidationProblemTitleBuildProperty] = "",
                    [GeneratorOptions.Keys.ModelBindingProblemTypeBuildProperty] = "",
                    [GeneratorOptions.Keys.ModelBindingProblemTitleBuildProperty] = ""
                }
            },
            localOptions: new TestAnalyzerConfigOptions
            {
                Options =
                {
                    [GeneratorOptions.Keys.AssignNameToEndpoint] = "",
                    [GeneratorOptions.Keys.ValidationProblemType] = "",
                    [GeneratorOptions.Keys.ValidationProblemTitle] = "",
                    [GeneratorOptions.Keys.ModelBindingProblemType] = "",
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

    private static IEnumerable<TestAnalyzerConfigOptionsProvider> ValidationProblemProviders()
    {
        // Validation problem type
        yield return new TestAnalyzerConfigOptionsProvider(
            globalOptions: new TestAnalyzerConfigOptions
            {
                Options =
                {
                    [GeneratorOptions.Keys.ValidationProblemTypeBuildProperty] = "global"
                }
            },
            localOptions: new TestAnalyzerConfigOptions(),
            friendlyName: "validation_type_global");

        yield return new TestAnalyzerConfigOptionsProvider(
            globalOptions: new TestAnalyzerConfigOptions(),
            localOptions: new TestAnalyzerConfigOptions
            {
                Options =
                {
                    [GeneratorOptions.Keys.ValidationProblemType] = "local"
                }
            },
            friendlyName: "validation_type_local");

        // Validation problem title
        yield return new TestAnalyzerConfigOptionsProvider(
            globalOptions: new TestAnalyzerConfigOptions
            {
                Options =
                {
                    [GeneratorOptions.Keys.ValidationProblemTitleBuildProperty] = "global"
                }
            },
            localOptions: new TestAnalyzerConfigOptions(),
            friendlyName: "validation_title_global");

        yield return new TestAnalyzerConfigOptionsProvider(
            globalOptions: new TestAnalyzerConfigOptions(),
            localOptions: new TestAnalyzerConfigOptions
            {
                Options =
                {
                    [GeneratorOptions.Keys.ValidationProblemTitle] = "local"
                }
            },
            friendlyName: "validation_title_local");

        // Model binding problem type
        yield return new TestAnalyzerConfigOptionsProvider(
            globalOptions: new TestAnalyzerConfigOptions
            {
                Options =
                {
                    [GeneratorOptions.Keys.ModelBindingProblemTypeBuildProperty] = "global"
                }
            },
            localOptions: new TestAnalyzerConfigOptions(),
            friendlyName: "model_binding_type_global");

        yield return new TestAnalyzerConfigOptionsProvider(
            globalOptions: new TestAnalyzerConfigOptions(),
            localOptions: new TestAnalyzerConfigOptions
            {
                Options =
                {
                    [GeneratorOptions.Keys.ModelBindingProblemType] = "local"
                }
            },
            friendlyName: "model_binding_type_local");

        // Model binding problem title
        yield return new TestAnalyzerConfigOptionsProvider(
            globalOptions: new TestAnalyzerConfigOptions
            {
                Options =
                {
                    [GeneratorOptions.Keys.ModelBindingProblemTitleBuildProperty] = "global"
                }
            },
            localOptions: new TestAnalyzerConfigOptions(),
            friendlyName: "model_binding_title_global");

        yield return new TestAnalyzerConfigOptionsProvider(
            globalOptions: new TestAnalyzerConfigOptions(),
            localOptions: new TestAnalyzerConfigOptions
            {
                Options =
                {
                    [GeneratorOptions.Keys.ModelBindingProblemTitle] = "local"
                }
            },
            friendlyName: "model_binding_title_local");
    }
}
