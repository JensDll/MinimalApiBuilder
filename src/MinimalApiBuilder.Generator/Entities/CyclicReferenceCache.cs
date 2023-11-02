namespace MinimalApiBuilder.Generator.Entities;

internal class CyclicReferenceCache<TKey, TValue> where TKey : class
{
    private readonly WeakReference<Entry?>[] _entries =
    {
        new(null),
        new(null),
        new(null),
        new(null),
        new(null)
    };

    public TValue GetOrCreateValue(TKey key, Func<TKey, TValue> valueFactory)
    {
        int newEntryIdx = -1;

        for (int i = 0; i < _entries.Length; ++i)
        {
            WeakReference<Entry?> entry = _entries[i];

            if (!entry.TryGetTarget(out Entry? target) || target is null)
            {
                newEntryIdx = newEntryIdx == -1 ? i : newEntryIdx;
                continue;
            }

            if (!Equals(target.Key, key))
            {
                continue;
            }

            return target.Value;
        }

        Entry newEntry = new(key, valueFactory(key));
        _entries[newEntryIdx == -1 ? 0 : newEntryIdx].SetTarget(newEntry);
        return newEntry.Value;
    }

    private sealed class Entry
    {
        public Entry(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public TKey Key { get; }

        public TValue Value { get; }
    }
}
