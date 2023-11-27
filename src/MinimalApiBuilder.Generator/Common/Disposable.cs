namespace MinimalApiBuilder.Generator.Common;

internal sealed class Disposable : IDisposable
{
    private readonly Action _onDispose;

    public Disposable(Action onDispose)
    {
        _onDispose = onDispose;
    }

    public static readonly Disposable Empty = new(static () =>
        { });

    public void Dispose()
    {
        _onDispose.Invoke();
    }
}
