using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Fixture.TestApi.Extensions;

internal static class StartupExtensions
{
    public static ILogger AddSerilogLogger(this ILoggingBuilder builder)
    {
        builder.ClearProviders();

        LoggerConfiguration loggerConfiguration = new();

        loggerConfiguration.WriteTo.Console(formatProvider: CultureInfo.InvariantCulture);

        ILogger logger = loggerConfiguration.CreateLogger();

        builder.AddSerilog(logger);
        builder.Services.AddSingleton(logger);

        return logger;
    }
}
