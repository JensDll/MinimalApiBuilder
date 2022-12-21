using Serilog;
using ILogger = Serilog.ILogger;

namespace Web.Extensions;

public static class StartupExtensions
{
    public static ILogger AddSerilogLogger(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();

        LoggerConfiguration loggerConfiguration = new();

        loggerConfiguration.WriteTo.Console();

        ILogger logger = loggerConfiguration.CreateLogger();

        builder.Logging.AddSerilog(logger);
        builder.Services.AddSingleton(logger);

        return logger;
    }
}
