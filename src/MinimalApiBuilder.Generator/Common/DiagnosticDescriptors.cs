using Microsoft.CodeAnalysis;

namespace MinimalApiBuilder.Generator.Common;

internal static class DiagnosticDescriptors
{
    private const string Prefix = "MINAPIBUILDER";
    private const string HelpLinkUri = "https://github.com/JensDll/MinimalApiBuilder";

    public static readonly DiagnosticDescriptor NullableValueTypeWillNotBeValidated = new(
        $"{Prefix}001",
        "Nullable value type will not be validated",
        "The nullable value type '{0}' will not take part in automatic request validation. Consider using a reference type.",
        "Usage",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        helpLinkUri: HelpLinkUri);
}
