using System.Diagnostics.Contracts;

namespace MinimalApiBuilder;

internal static class NullableExtensions
{
    [Pure]
    public static T ThrowIfNull<T>(this T? type, string message) where T : class =>
        type ?? throw new InvalidOperationException(message);

    [Pure]
    public static T ThrowIfNull<T>(this T? type, string message) where T : struct =>
        type ?? throw new InvalidOperationException(message);
}
