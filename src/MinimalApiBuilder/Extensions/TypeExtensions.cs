using System.Diagnostics.Contracts;
using System.Reflection;

namespace MinimalApiBuilder;

internal static class TypeExtensions
{
    [Pure]
    public static T GetCustomAttribute<T>(this Type type) where T : Attribute =>
        (T?)type.GetCustomAttribute(typeof(T)) ??
        throw new InvalidOperationException(
            $"Type \"{type}\" does not have required attribute \"{typeof(T)}\"");
}
