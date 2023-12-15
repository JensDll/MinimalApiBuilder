using Microsoft.Extensions.Logging;

namespace MinimalApiBuilder.UnitTests.Infrastructure;

/// <summary>
/// Test logger as mock helper.
/// </summary>
public abstract class TestLogger : ILogger
{
    /// <summary>
    /// For mocking.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="message"></param>
    public abstract void Log(LogLevel logLevel, string message);

    /// <summary>
    /// For mocking.
    /// </summary>
    /// <param name="state"></param>
    /// <typeparam name="TState"></typeparam>
    /// <returns></returns>
    public abstract IDisposable BeginScope<TState>(TState state) where TState : notnull;

    /// <inheritdoc />
    void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state,
        Exception? exception, Func<TState, Exception?, string> formatter)
    {
        Log(logLevel, formatter(state, exception));
    }

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel) => true;
}
