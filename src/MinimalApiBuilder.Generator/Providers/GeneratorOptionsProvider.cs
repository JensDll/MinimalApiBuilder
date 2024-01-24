using Microsoft.CodeAnalysis;
using MinimalApiBuilder.Generator.Entities;

namespace MinimalApiBuilder.Generator.Providers;

internal static class GeneratorOptionsProvider
{
    public const string TrackingName = nameof(GeneratorOptionsProvider);

    public static IncrementalValueProvider<GeneratorOptions> ForGeneratorOptions(
        this IncrementalGeneratorInitializationContext context) =>
        context.AnalyzerConfigOptionsProvider.Select(static (provider, _) => new GeneratorOptions(provider))
            .WithTrackingName(TrackingName);
}
