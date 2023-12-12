using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MinimalApiBuilder.Middleware;

namespace MinimalApiBuilder.UnitTests.Infrastructure;

internal static class StaticFilesTestServer
{
    public static Task<IHost> Create()
    {
        return Create(static app =>
        {
            app.UseCompressedStaticFiles();
        }, static services =>
        {
            services.AddCompressedStaticFileMiddleware();
        });
    }

    public static Task<IHost> Create(CompressedStaticFileOptions options)
    {
        return Create(static app =>
        {
            app.UseCompressedStaticFiles();
        }, services =>
        {
            services.AddCompressedStaticFileMiddleware(options);
        });
    }

    private static async Task<IHost> Create(
        Action<IApplicationBuilder> configureApp,
        Action<IServiceCollection> configureServices)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddInMemoryCollection([new KeyValuePair<string, string?>(WebHostDefaults.WebRootKey, "static")])
            .Build();

        IHost host = new HostBuilder()
            .ConfigureWebHost(builder => builder
                .UseTestServer()
                .UseConfiguration(configuration)
                .Configure(configureApp)
                .ConfigureServices(configureServices))
            .Build();

        await host.StartAsync();

        return host;
    }
}
