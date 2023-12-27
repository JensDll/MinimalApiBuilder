using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MinimalApiBuilder.Middleware;

namespace MinimalApiBuilder.UnitTests.Infrastructure;

internal sealed class StaticFilesTestServer : IDisposable
{
    private readonly IHost _host;
    private readonly TestServer _server;

    public StaticFilesTestServer(IHost host, TestServer server, HttpClient client)
    {
        _host = host;
        _server = server;
        Client = client;
    }

    public HttpClient Client { get; }

    public static Task<StaticFilesTestServer> CreateAsync()
    {
        return CreateAsync(DefaultConfigureApp, DefaultConfigureServices);
    }

    public static Task<StaticFilesTestServer> CreateAsync(CompressedStaticFileOptions staticFileOptions)
    {
        return CreateAsync(DefaultConfigureApp, DefaultConfigureServices(staticFileOptions));
    }

    public static Task<StaticFilesTestServer> CreateAsync(Action<IApplicationBuilder> configureApp)
    {
        return GetHostBuilder(configureApp, DefaultConfigureServices).BuildStaticFilesTestServerAsync();
    }

    public static Task<StaticFilesTestServer> CreateAsync(Action<IApplicationBuilder> configureApp, ILogger logger)
    {
        return GetHostBuilder(configureApp, DefaultConfigureServices)
            .ConfigureTestLoggingProvider(logger)
            .BuildStaticFilesTestServerAsync();
    }

    public static Task<StaticFilesTestServer> CreateAsync(Action<IServiceCollection> configureServices)
    {
        return GetHostBuilder(DefaultConfigureApp, configureServices).BuildStaticFilesTestServerAsync();
    }

    public static Task<StaticFilesTestServer> CreateAsync(Action<IServiceCollection> configureServices, ILogger logger)
    {
        return GetHostBuilder(DefaultConfigureApp, configureServices)
            .ConfigureTestLoggingProvider(logger)
            .BuildStaticFilesTestServerAsync();
    }

    public static Task<StaticFilesTestServer> CreateAsync(Action<IApplicationBuilder> configureApp,
        Action<IServiceCollection> configureServices)
    {
        return GetHostBuilder(configureApp, configureServices)
            .BuildStaticFilesTestServerAsync();
    }

    public static Task<StaticFilesTestServer> CreateAsync(Action<IApplicationBuilder> configureApp,
        Action<IServiceCollection> configureServices, ILogger logger)
    {
        return GetHostBuilder(configureApp, configureServices)
            .ConfigureTestLoggingProvider(logger)
            .BuildStaticFilesTestServerAsync();
    }

    public static void DefaultConfigureApp(IApplicationBuilder app)
    {
        app.UseCompressedStaticFiles();
    }

    public static void DefaultConfigureServices(IServiceCollection services)
    {
        services.AddCompressedStaticFileMiddleware();
    }

    public static Action<IServiceCollection> DefaultConfigureServices(CompressedStaticFileOptions staticFileOptions)
    {
        return services =>
        {
            services.AddCompressedStaticFileMiddleware(staticFileOptions);
        };
    }

    private static IHostBuilder GetHostBuilder(
        Action<IApplicationBuilder> configureApp,
        Action<IServiceCollection> configureServices)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddInMemoryCollection([new KeyValuePair<string, string?>(WebHostDefaults.WebRootKey, "static")])
            .Build();

        return new HostBuilder()
            .ConfigureWebHost(webHostBuilder => webHostBuilder
                .UseTestServer()
                .UseConfiguration(configuration)
                .Configure(configureApp)
                .ConfigureServices(configureServices));
    }

    public void Dispose()
    {
        _host.Dispose();
        _server.Dispose();
        Client.Dispose();
    }
}
