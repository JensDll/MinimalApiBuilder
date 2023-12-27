using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MinimalApiBuilder.Middleware;

namespace MinimalApiBuilder.UnitTests.Infrastructure;

internal static class HostBuilderExtensions
{
    public static IWebHostBuilder ConfigureDefaults(this IWebHostBuilder webHostBuilder)
    {
        webHostBuilder.Configure(StaticFilesTestServer.DefaultConfigureApp);
        webHostBuilder.ConfigureServices(StaticFilesTestServer.DefaultConfigureServices);

        return webHostBuilder;
    }

    public static IWebHostBuilder ConfigureDefaults(this IWebHostBuilder webHostBuilder,
        CompressedStaticFileOptions staticFileOptions)
    {
        webHostBuilder.Configure(StaticFilesTestServer.DefaultConfigureApp);
        webHostBuilder.ConfigureServices(StaticFilesTestServer.DefaultConfigureServices(staticFileOptions));

        return webHostBuilder;
    }

    public static IHostBuilder ConfigureTestLoggingProvider(this IHostBuilder hostBuilder, ILogger logger)
    {
        hostBuilder.ConfigureLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddProvider(new TestLoggerProvider(logger));
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        return hostBuilder;
    }

    public static async Task<StaticFilesTestServer> BuildStaticFilesTestServerAsync(this IHostBuilder hostBuilder)
    {
        IHost host = hostBuilder.Build();

        await host.StartAsync();

        TestServer server = host.GetTestServer();
        HttpClient client = server.CreateClient();

        return new StaticFilesTestServer(host, server, client);
    }
}
