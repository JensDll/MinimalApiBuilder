namespace MinimalApiBuilder.Generator.UnitTests;

internal static class DictionaryExtensions
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
}
