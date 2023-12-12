using Microsoft.CodeAnalysis;

namespace MinimalApiBuilder.Generator.UnitTests.Infrastructure;

internal static class TestExtensions
{
    public static Dictionary<TKey, TValue> AddAndReturn<TKey, TValue>(this Dictionary<TKey, TValue> dictionary,
        TKey key, TValue value) where TKey : notnull
    {
        dictionary.Add(key, value);
        return dictionary;
    }

    public static Dictionary<TKey, TValue> ChangeAndReturn<TKey, TValue>(this Dictionary<TKey, TValue> dictionary,
        TKey key, TValue value) where TKey : notnull
    {
        dictionary[key] = value;
        return dictionary;
    }

    public static IEnumerable<Diagnostic> WarningsOrWorse(this IEnumerable<Diagnostic> diagnostics)
    {
        return diagnostics.Where(static diagnostic => diagnostic.Severity >= DiagnosticSeverity.Warning);
    }
}
