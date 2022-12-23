using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace MinimalApiBuilder;

internal static class TypeExtensions
{
    [Pure]
    public static T GetCustomAttribute<T>(this Type type) where T : Attribute
    {
        return (T?)type.GetCustomAttribute(typeof(T)) ??
               throw new InvalidOperationException(
                   $"Type \"{type}\" does not have required attribute \"{typeof(T)}\"");
    }

    [Pure]
    public static bool TryGetAttribute<T>(this Type type, [NotNullWhen(true)] out T? attribute)
        where T : Attribute
    {
        return (attribute = (T?)type.GetCustomAttribute(typeof(T))) is not null;
    }
}
