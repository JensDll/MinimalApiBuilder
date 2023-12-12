using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MinimalApiBuilder.Middleware;
using MinimalApiBuilder.UnitTests.Infrastructure;
using NSubstitute;
using NUnit.Framework;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace MinimalApiBuilder.UnitTests.Middleware;

internal sealed class CompressedStaticFileMiddlewareTests
{
    private static readonly Uri s_uri = new("/data.txt", UriKind.Relative);

    [Test]
    public async Task NotFound_And_Logs_Warning_When_WebRootPath_Is_Missing()
    {
        var logger = Substitute.For<TestLogger>();

        using var host = new HostBuilder()
            .ConfigureWebHost(webHostBuilder => webHostBuilder
                .UseTestServer()
                .ConfigureLogging(builder =>
                {
                    builder.ClearProviders();
                    builder.AddProvider(new TestLoggerProvider(logger));
                })
                .ConfigureServices(static services =>
                {
                    services.AddCompressedStaticFileMiddleware();
                })
                .Configure(static app =>
                {
                    app.UseCompressedStaticFiles();
                }))
            .Build();

        await host.StartAsync();

        using var server = host.GetTestServer();
        using var client = server.CreateClient();

        using HttpResponseMessage response = await client.GetAsync(s_uri);

        logger.Received(1).Log(Arg.Is(LogLevel.Warning), Arg.Is<string>(value =>
            value.Contains("The WebRootPath was not found:")
            && value.Contains("Compressed static files may be unavailable")));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task NotFound_When_SendFile_Throws()
    {
        var mockSendFile = Substitute.For<IHttpResponseBodyFeature>();
        mockSendFile.When(s => s.SendFileAsync(Arg.Any<string>(),
                Arg.Any<long>(), Arg.Any<long>(), Arg.Any<CancellationToken>()))
            .Throw<FileNotFoundException>();
        mockSendFile.Stream.Returns(Stream.Null);

        using var host = new HostBuilder()
            .ConfigureWebHost(webHostBuilder => webHostBuilder
                .UseTestServer()
                .ConfigureServices(static services =>
                {
                    services.AddCompressedStaticFileMiddleware();
                })
                .Configure(app =>
                {
                    app.Use(next => async context =>
                    {
                        context.Features.Set(mockSendFile);
                        await next(context);
                    });
                    app.UseCompressedStaticFiles();
                })
                .UseWebRoot("static"))
            .Build();

        await host.StartAsync();

        using var server = host.GetTestServer();
        using var client = server.CreateClient();

        using HttpResponseMessage response = await client.GetAsync(s_uri);

        await mockSendFile.Received(1).SendFileAsync(Arg.Any<string>(),
            Arg.Any<long>(), Arg.Any<long>(), Arg.Any<CancellationToken>());

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
