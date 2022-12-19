using System.Diagnostics.Contracts;

namespace MinimalApiBuilder;

internal static class NullableExtensions
{
    [Pure]
    public static T ThrowIfNull<T>(this T? type, string message) =>
        type ?? throw new InvalidOperationException(message);
}
