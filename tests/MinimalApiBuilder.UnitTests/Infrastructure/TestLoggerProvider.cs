using Microsoft.Extensions.Logging;

namespace MinimalApiBuilder.UnitTests.Infrastructure;

internal sealed class TestLoggerProvider : ILoggerProvider
{
    private readonly ILogger _logger;

    public TestLoggerProvider(ILogger logger)
    {
        _logger = logger;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return _logger;
    }

    public void Dispose() { }
}
