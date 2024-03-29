﻿using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using MinimalApiBuilder.Middleware;
using MinimalApiBuilder.UnitTests.Infrastructure;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace MinimalApiBuilder.UnitTests.Middleware;

internal sealed class CompressedStaticFileMiddlewareTests
{
    [Test]
    public async Task NotFound_And_Logs_Warning_When_WebRootPath_Is_Missing()
    {
        var logger = Substitute.For<TestLogger>();

        using StaticFilesTestServer server = await new HostBuilder()
            .ConfigureTestLoggingProvider(logger)
            .ConfigureWebHost(builder => builder
                .UseTestServer()
                .ConfigureDefaults())
            .BuildStaticFilesTestServerAsync();

        using HttpResponseMessage response = await server.Client.GetAsync(StaticUri.DataTxtUri);

        logger.Received(2).Log(Arg.Is(LogLevel.Warning), Arg.Is<string>(static message =>
            message.Contains("The WebRootPath was not found:")
            && message.Contains("Compressed static files may be unavailable")));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task NotFound_When_SendFile_Throws()
    {
        var feature = Substitute.For<IHttpResponseBodyFeature>();
        feature.SendFileAsync(
            path: Arg.Any<string>(),
            offset: Arg.Any<long>(),
            count: Arg.Any<long>(),
            cancellationToken: Arg.Any<CancellationToken>()).ThrowsAsync<FileNotFoundException>();
        feature.Stream.Returns(Stream.Null);

        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync(configureApp: builder =>
        {
            builder.Use(next => context =>
            {
                context.Features.Set(feature);
                return next(context);
            });
            builder.UseCompressedStaticFiles();
        });

        using HttpResponseMessage response = await server.Client.GetAsync(StaticUri.DataTxtUri);
        byte[] responseContent = await response.Content.ReadAsByteArrayAsync();

        await feature.Received(1).SendFileAsync(Arg.Any<string>(), Arg.Any<long>(), Arg.Any<long>(),
            Arg.Any<CancellationToken>());

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(responseContent, Is.Empty);
        });
    }

    [Test]
    public async Task OperationCanceledException_Caught_And_Logged()
    {
        var feature = Substitute.For<IHttpResponseBodyFeature>();
        feature.SendFileAsync(
            path: Arg.Any<string>(),
            offset: Arg.Any<long>(),
            count: Arg.Any<long>(),
            cancellationToken: Arg.Any<CancellationToken>()).ThrowsAsync<OperationCanceledException>();

        var logger = Substitute.For<TestLogger>();

        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync(configureApp: builder =>
        {
            builder.Use(next => context =>
            {
                context.Features.Set(feature);
                return next(context);
            });
            builder.UseCompressedStaticFiles();
        }, logger);

        using HttpResponseMessage response = await server.Client.GetAsync(StaticUri.DataTxtUri);
        byte[] responseContent = await response.Content.ReadAsByteArrayAsync();

        await feature.Received(1).SendFileAsync(Arg.Any<string>(), Arg.Any<long>(), Arg.Any<long>(),
            Arg.Any<CancellationToken>());

        logger.Received(1).Log(Arg.Is(LogLevel.Debug), Arg.Is<string>(
            static message => message == "The file transmission was cancelled"));

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseContent, Is.Empty);
        });
    }

    [Test]
    public async Task LastModified_Trims_To_Second_Precision()
    {
        using PhysicalFileProvider provider = new(Path.Combine(TestContext.CurrentContext.TestDirectory, "static"));
        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync(new CompressedStaticFileOptions
        {
            FileProvider = provider
        });

        IFileInfo fileInfo = provider.GetFileInfo(StaticUri.DataTxtUri.ToString());

        Assert.That(fileInfo.Exists, Is.True);

        DateTimeOffset last = fileInfo.LastModified;
        DateTimeOffset trimmed = new DateTimeOffset(last.Year, last.Month, last.Day, last.Hour,
            last.Minute, last.Second, last.Offset).ToUniversalTime();

        using HttpResponseMessage response = await server.Client.GetAsync(StaticUri.DataTxtUri);

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content.Headers.LastModified, Is.Not.EqualTo(last));
            Assert.That(response.Content.Headers.LastModified, Is.EqualTo(trimmed));
        });
    }

    private static readonly char[] s_etagChars =
    {
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
        'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
        'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '=', '-'
    };

    [Test]
    public async Task Etag_Is_Combination_Of_LastModified_And_ContentLength()
    {
        using PhysicalFileProvider provider = new(Path.Combine(TestContext.CurrentContext.TestDirectory, "static"));
        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync(new CompressedStaticFileOptions
        {
            FileProvider = provider
        });

        IFileInfo fileInfo = provider.GetFileInfo(StaticUri.DataTxtUri.ToString());

        Assert.That(fileInfo.Exists, Is.True);

        using HttpResponseMessage response = await server.Client.GetAsync(StaticUri.DataTxtUri);

        Assert.Multiple(() =>
        {
            Assert.That(response.Headers.ETag, Is.Not.Null);
            Assert.That(response.Content.Headers.ContentLength, Is.Not.Null);
            Assert.That(response.Content.Headers.LastModified, Is.Not.Null);
        });

        long etagHash = response.Content.Headers.LastModified!.Value.ToFileTime() ^
                        response.Content.Headers.ContentLength!.Value;

        string etag = string.Create(13, etagHash, static (span, hash) =>
        {
            span[0] = '"';
            span[1] = s_etagChars[hash & 63];
            span[2] = s_etagChars[(hash >> 6) & 63];
            span[3] = s_etagChars[(hash >> 12) & 63];
            span[4] = s_etagChars[(hash >> 18) & 63];
            span[5] = s_etagChars[(hash >> 24) & 63];
            span[6] = s_etagChars[(hash >> 30) & 63];
            span[7] = s_etagChars[(hash >> 36) & 63];
            span[8] = s_etagChars[(hash >> 42) & 63];
            span[9] = s_etagChars[(hash >> 48) & 63];
            span[10] = s_etagChars[(hash >> 54) & 63];
            span[11] = s_etagChars[(hash >> 60) & 63];
            span[12] = '"';
        });

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Headers.ETag!.Tag, Is.EqualTo(etag));
        });
    }

    private static IEnumerable<CompressedStaticFileOptions> NullValueOptions()
    {
        yield return new CompressedStaticFileOptions
        {
            FileProvider = null
        };

        yield return new CompressedStaticFileOptions
        {
            ContentTypeProvider = null!
        };

        yield return new CompressedStaticFileOptions
        {
            RequestPath = null
        };
    }

    [TestCaseSource(nameof(NullValueOptions))]
    public async Task Null_Values_In_Options_Allowed(CompressedStaticFileOptions options)
    {
        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync(options);
        using HttpResponseMessage response = await server.Client.GetAsync(StaticUri.DataTxtUri);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [TestCase("", "/data.txt", "data")]
    [TestCase("", "/sub/data.js", """const data = "Hello, World!";""")]
    [TestCase("/something", "/something/sub/data.js", """const data = "Hello, World!";""")]
    public async Task Requested_Files_Are_Found(string requestPath, string requestUri, string expectedContent)
    {
        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync(new CompressedStaticFileOptions
        {
            RequestPath = new PathString(requestPath)
        });

        Uri uri = new(requestUri, UriKind.Relative);
        using HttpResponseMessage response = await server.Client.GetAsync(uri);
        string responseContent = await response.Content.ReadAsStringAsync();

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content.Headers.ContentLength, Is.EqualTo(expectedContent.Length));
            Assert.That(responseContent, Is.EqualTo(expectedContent));
        });
    }

    [TestCase("", "/data.txt")]
    [TestCase("", "/sub/data.js")]
    [TestCase("/something", "/something/sub/data.js")]
    public async Task HEAD_Requested_Files_Are_Found_Without_Body(string requestPath, string requestUri)
    {
        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync(new CompressedStaticFileOptions
        {
            RequestPath = new PathString(requestPath)
        });

        Uri uri = new(requestUri, UriKind.Relative);
        using HttpRequestMessage request = new(HttpMethod.Head, uri);
        using HttpResponseMessage response = await server.Client.SendAsync(request);

        byte[] responseContent = await response.Content.ReadAsByteArrayAsync();

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content.Headers.LastModified, Is.Not.Null);
            Assert.That(response.Headers.ETag, Is.Not.Null);
            Assert.That(response.Headers.AcceptRanges.ToString(), Is.EqualTo("bytes"));
            Assert.That(response.Content.Headers.ContentType, Is.Not.Null);
            Assert.That(response.Content.Headers.ContentLength, Is.Not.Null);
            Assert.That(responseContent, Is.Empty);
        });
    }

    [TestCase(400)]
    [TestCase(500)]
    public async Task Does_Not_Override_Non_Default_Status_Code(int statusCode)
    {
        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync(configureApp: builder =>
        {
            builder.Use(next => context =>
            {
                context.Response.StatusCode = statusCode;
                return next(context);
            });
            builder.UseCompressedStaticFiles();
        });

        using HttpResponseMessage response = await server.Client.GetAsync(StaticUri.DataTxtUri);

        Assert.That((int)response.StatusCode, Is.EqualTo(statusCode));
    }

    [Test]
    public async Task OnPrepareResponse_Is_Called()
    {
        bool called = false;

        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync(new CompressedStaticFileOptions
        {
            OnPrepareResponse = _ => called = true
        });

        using HttpResponseMessage response = await server.Client.GetAsync(StaticUri.DataTxtUri);

        Assert.That(called, Is.True);
    }

    [Test]
    public async Task OnPrepareResponseAsync_Is_Called()
    {
        bool called = false;

        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync(new CompressedStaticFileOptions
        {
            OnPrepareResponseAsync = _ =>
            {
                called = true;
                return Task.CompletedTask;
            }
        });

        using HttpResponseMessage response = await server.Client.GetAsync(StaticUri.DataTxtUri);

        Assert.That(called, Is.True);
    }

    [Test]
    public async Task OnPrepareResponse_Is_Called_Before_OnPrepareResponseAsync()
    {
        bool syncCalled = false;
        bool asyncCalled = false;

        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync(new CompressedStaticFileOptions
        {
            OnPrepareResponse = _ =>
            {
                Assert.Multiple(() =>
                {
                    Assert.That(syncCalled, Is.False);
                    Assert.That(asyncCalled, Is.False);
                    syncCalled = true;
                });
            },
            OnPrepareResponseAsync = _ =>
            {
                Assert.Multiple(() =>
                {
                    Assert.That(syncCalled, Is.True);
                    Assert.That(asyncCalled, Is.False);
                    asyncCalled = true;
                });
                return Task.CompletedTask;
            }
        });

        using HttpResponseMessage response = await server.Client.GetAsync(StaticUri.DataTxtUri);

        Assert.Multiple(() =>
        {
            Assert.That(syncCalled, Is.True);
            Assert.That(asyncCalled, Is.True);
        });
    }

    [Test]
    public async Task OnPrepareResponse_Is_Called_With_Correct_Arguments()
    {
        int timesCalled = 0;

        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync(new CompressedStaticFileOptions
        {
            OnPrepareResponse = context =>
            {
                ++timesCalled;
                Assert.Multiple(() =>
                {
                    Assert.That(context.Context, Is.Not.Null);
                    Assert.That(context.Filename, Is.EqualTo("data.txt"));
                    Assert.That(context.ContentCoding, timesCalled == 1 ? Is.Null : Is.EqualTo("br"));
                });
            }
        });

        Assert.That(timesCalled, Is.EqualTo(0));

        using HttpRequestMessage normalRequest = new(HttpMethod.Get, StaticUri.DataTxtUri);
        using HttpResponseMessage normalResponse = await server.Client.SendAsync(normalRequest);
        string normalContent = await normalResponse.Content.ReadAsStringAsync();
        Assert.That(timesCalled, Is.EqualTo(1));

        using HttpRequestMessage compressedRequest = new(HttpMethod.Get, StaticUri.DataTxtUri);
        compressedRequest.Headers.Add(HeaderNames.AcceptEncoding, "br");
        using HttpResponseMessage compressedResponse = await server.Client.SendAsync(compressedRequest);
        string compressedContent = await compressedResponse.Content.ReadAsStringAsync();
        Assert.That(timesCalled, Is.EqualTo(2));

        Assert.Multiple(() =>
        {
            Assert.That(normalResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(normalContent, Is.EqualTo("data"));
            Assert.That(compressedResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(compressedContent, Is.EqualTo("br data"));
        });
    }

    [Test]
    public async Task OnPrepareResponseAsync_Is_Called_With_Correct_Arguments()
    {
        int timesCalled = 0;

        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync(new CompressedStaticFileOptions
        {
            OnPrepareResponseAsync = context =>
            {
                ++timesCalled;
                Assert.Multiple(() =>
                {
                    Assert.That(context.Context, Is.Not.Null);
                    Assert.That(context.Filename, Is.EqualTo("data.txt"));
                    Assert.That(context.ContentCoding, timesCalled == 1 ? Is.Null : Is.EqualTo("br"));
                });
                return Task.CompletedTask;
            }
        });

        Assert.That(timesCalled, Is.EqualTo(0));

        using HttpRequestMessage normalRequest = new(HttpMethod.Get, StaticUri.DataTxtUri);
        using HttpResponseMessage normalResponse = await server.Client.SendAsync(normalRequest);
        string normalContent = await normalResponse.Content.ReadAsStringAsync();
        Assert.That(timesCalled, Is.EqualTo(1));

        using HttpRequestMessage compressedRequest = new(HttpMethod.Get, StaticUri.DataTxtUri);
        compressedRequest.Headers.Add(HeaderNames.AcceptEncoding, "br");
        using HttpResponseMessage compressedResponse = await server.Client.SendAsync(compressedRequest);
        string compressedContent = await compressedResponse.Content.ReadAsStringAsync();
        Assert.That(timesCalled, Is.EqualTo(2));

        Assert.Multiple(() =>
        {
            Assert.That(normalResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(normalContent, Is.EqualTo("data"));
            Assert.That(compressedResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(compressedContent, Is.EqualTo("br data"));
        });
    }
}
