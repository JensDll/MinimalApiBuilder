using Microsoft.Extensions.Logging;

namespace MinimalApiBuilder.UnitTests.Infrastructure;

/// <inheritdoc />
public abstract class TestLogger : ILogger
{
    /// <summary>
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="message"></param>
    public abstract void Log(LogLevel logLevel, string message);

    /// <inheritdoc />
    public abstract IDisposable BeginScope<TState>(TState state) where TState : notnull;

    void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state,
        Exception? exception, Func<TState, Exception?, string> formatter)
    {
        Log(logLevel, formatter(state, exception));
    }

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel) => true;
}
