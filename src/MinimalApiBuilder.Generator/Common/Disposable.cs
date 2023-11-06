namespace MinimalApiBuilder.Generator.Common;

internal class Disposable : IDisposable
{
    private Action? _onDispose;

    public Disposable(Action onDispose)
    {
        _onDispose = onDispose;
    }

    private Disposable()
    { }

    public static readonly Disposable Empty = new();

    public void Dispose()
    {
        _onDispose?.Invoke();
        _onDispose = null;
    }
}
