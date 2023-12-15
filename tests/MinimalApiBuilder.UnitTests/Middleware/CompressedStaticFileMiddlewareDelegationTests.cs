using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using MinimalApiBuilder.Middleware;
using MinimalApiBuilder.UnitTests.Infrastructure;
using NSubstitute;
using NUnit.Framework;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace MinimalApiBuilder.UnitTests.Middleware;

internal sealed class CompressedStaticFileMiddlewareDelegationTests
{
    private static readonly Uri s_uri = new("/data.txt", UriKind.Relative);

    [Test]
    public async Task Endpoint_With_RequestDelegate_Delegates_Request()
    {
        var logger = Substitute.For<TestLogger>();

        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync(static builder =>
        {
            builder.UseRouting();
            builder.Use(static next => context =>
            {
                context.SetEndpoint(new Endpoint(requestDelegate: static httpContext =>
                {
                    httpContext.Response.ContentType = "text/endpoint";
                    return Task.CompletedTask;
                }, new EndpointMetadataCollection(), "Endpoint"));
                return next(context);
            });
            builder.UseCompressedStaticFiles();
            builder.UseEndpoints(_ =>
                { });
        }, static services =>
        {
            services.AddCompressedStaticFileMiddleware();
            services.AddRouting();
        }, logger);

        using HttpResponseMessage response = await server.Client.GetAsync(s_uri);

        logger.Received(1).Log(LogLevel.Debug,
            Arg.Is<string>(message => message == "Skipping as the request already matches an endpoint"));

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content.Headers.ContentType!.MediaType, Is.EqualTo("text/endpoint"));
        });
    }

    [Test]
    public async Task Endpoint_With_Null_RequestDelegate_Does_Not_Delegate_Request()
    {
        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync(static builder =>
        {
            builder.UseRouting();
            builder.Use(static next => context =>
            {
                context.SetEndpoint(new Endpoint(requestDelegate: null, new EndpointMetadataCollection(),
                    "NullEndpoint"));
                return next(context);
            });
            builder.UseCompressedStaticFiles();
            builder.UseEndpoints(_ =>
                { });
        }, static services =>
        {
            services.AddCompressedStaticFileMiddleware();
            services.AddRouting();
        });

        using HttpResponseMessage response = await server.Client.GetAsync(s_uri);

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content.Headers.ContentType!.MediaType, Is.EqualTo("text/plain"));
        });
    }

    private static readonly string[] s_missingFiles =
    [
        "/foo.txt",
        "/bar.js",
        "/baz.css"
    ];

    private static readonly string[] s_existingFiles =
    [
        "/data.txt",
        "/range.txt",
        "/sub/data.js"
    ];

    private static readonly string[] s_files = [.. s_missingFiles, .. s_existingFiles];

    [TestCaseSource(nameof(s_missingFiles))]
    public Task Get_No_Match_Delegates_Request(string uri) => DelegatesRequest(HttpMethod.Get, uri);

    [TestCaseSource(nameof(s_missingFiles))]
    public Task Head_No_Match_Delegates_Request(string uri) => DelegatesRequest(HttpMethod.Head, uri);

    [TestCaseSource(nameof(s_files))]
    public Task Unknown_Verb_Delegates_Request(string uri) => DelegatesRequest(new HttpMethod("VERB"), uri);

    [TestCaseSource(nameof(s_files))]
    public Task Post_Delegates_Request(string uri) => DelegatesRequest(HttpMethod.Post, uri);

    [TestCaseSource(nameof(s_files))]
    public Task Put_Delegates_Request(string uri) => DelegatesRequest(HttpMethod.Put, uri);

    [TestCaseSource(nameof(s_files))]
    public Task Delete_Delegates_Request(string uri) => DelegatesRequest(HttpMethod.Delete, uri);

    [TestCaseSource(nameof(s_files))]
    public Task Connect_Delegates_Request(string uri) => DelegatesRequest(HttpMethod.Connect, uri);

    [TestCaseSource(nameof(s_files))]
    public Task Options_Delegates_Request(string uri) => DelegatesRequest(HttpMethod.Options, uri);

    [TestCaseSource(nameof(s_files))]
    public Task Trace_Delegates_Request(string uri) => DelegatesRequest(HttpMethod.Trace, uri);

    [TestCaseSource(nameof(s_files))]
    public Task Patch_Delegates_Request(string uri) => DelegatesRequest(HttpMethod.Patch, uri);

    [TestCaseSource(nameof(s_existingFiles))]
    public Task Unmatched_RequestPath_Delegates_Request(string uri) => DelegatesRequest(HttpMethod.Get, uri,
        new CompressedStaticFileOptions
        {
            RequestPath = "/foo"
        });

    [TestCaseSource(nameof(s_files))]
    public Task Unconfigured_Content_Type_Delegates_Request(string uri) => DelegatesRequest(HttpMethod.Get, uri,
        new CompressedStaticFileOptions
        {
            ContentTypeProvider = FalseContentTypeProvider.Instance
        });

    [TestCaseSource(nameof(s_existingFiles))]
    public async Task Unconfigured_Content_Type_With_ServeUnknownFileTypes_Does_Not_Delegate_Request(string uri)
    {
        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync(new CompressedStaticFileOptions
        {
            ContentTypeProvider = FalseContentTypeProvider.Instance,
            ServeUnknownFileTypes = true
        });

        using HttpRequestMessage request = new(HttpMethod.Get, new Uri(uri, UriKind.Relative));
        using HttpResponseMessage response = await server.Client.SendAsync(request);

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Headers.ETag, Is.Not.Null);
            Assert.That(response.Content.Headers.ContentLength, Is.GreaterThan(0));
        });
    }

    private static Task DelegatesRequest(HttpMethod method, string requestUri)
    {
        return DelegatesRequest(method, requestUri, new CompressedStaticFileOptions());
    }

    private static async Task DelegatesRequest(HttpMethod method, string requestUri,
        CompressedStaticFileOptions options)
    {
        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync(options);

        Uri uri = new(requestUri, UriKind.Relative);
        using HttpRequestMessage request = new(method, uri);
        using HttpResponseMessage response = await server.Client.SendAsync(request);

        Assert.Multiple(() =>
        {
            Assert.That(response.Content.Headers.LastModified, Is.Null);
            Assert.That(response.Headers.ETag, Is.Null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        });
    }

    private sealed class FalseContentTypeProvider : IContentTypeProvider
    {
        public static readonly FalseContentTypeProvider Instance = new();

        public bool TryGetContentType(string subpath, [MaybeNullWhen(false)] out string contentType)
        {
            contentType = null;
            return false;
        }
    }
}
