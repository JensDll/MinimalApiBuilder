using System.Net;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Net.Http.Headers;
using MinimalApiBuilder.UnitTests.Infrastructure;
using NUnit.Framework;

namespace MinimalApiBuilder.UnitTests.Middleware;

internal sealed class PreconditionTests
{
    private static readonly Uri s_uri = new("/data.txt", UriKind.Relative);

    [Test]
    public async Task IfMatch_412_When_Not_Listed()
    {
        using var host = await StaticFilesTestServer.Create();
        using var server = host.GetTestServer();
        using var client = server.CreateClient();

        using HttpRequestMessage request = new(HttpMethod.Get, s_uri);
        request.Headers.Add(HeaderNames.IfMatch, "\"not-matching\"");

        using HttpResponseMessage response = await client.SendAsync(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.PreconditionFailed));
    }

    [Test]
    public async Task IfMatch_200_When_Listed([Values] bool alone)
    {
        using var host = await StaticFilesTestServer.Create();
        using var server = host.GetTestServer();
        using var client = server.CreateClient();

        using HttpResponseMessage original = await client.GetAsync(s_uri);
        string etag = original.Headers.ETag!.Tag;

        using HttpRequestMessage request = new(HttpMethod.Get, s_uri);
        request.Headers.Add(HeaderNames.IfMatch, alone ? etag : $"\"tag1\", {etag}, \"tag2\"");

        using HttpResponseMessage response = await client.SendAsync(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task IfMatch_200_When_Star()
    {
        using var host = await StaticFilesTestServer.Create();
        using var server = host.GetTestServer();
        using var client = server.CreateClient();

        using HttpResponseMessage original = await client.GetAsync(s_uri);

        using HttpRequestMessage request = new(HttpMethod.Get, s_uri);
        request.Headers.Add(HeaderNames.IfMatch, "*");

        using HttpResponseMessage response = await client.SendAsync(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task IfNoneMatch_200_When_Not_Listed()
    {
        using var host = await StaticFilesTestServer.Create();
        using var server = host.GetTestServer();
        using var client = server.CreateClient();

        using HttpRequestMessage request = new(HttpMethod.Get, s_uri);
        request.Headers.Add(HeaderNames.IfNoneMatch, "\"not-matching\"");

        using HttpResponseMessage response = await client.SendAsync(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task IfNoneMatch_304_When_Listed([Values] bool alone, [Values] bool weak)
    {
        using var host = await StaticFilesTestServer.Create();
        using var server = host.GetTestServer();
        using var client = server.CreateClient();

        using HttpResponseMessage original = await client.GetAsync(s_uri);
        string etag = weak ? $"W/{original.Headers.ETag!.Tag}" : original.Headers.ETag!.Tag;

        Assert.That(original.Headers.ETag.IsWeak, Is.False);

        using HttpRequestMessage request = new(HttpMethod.Get, s_uri);
        request.Headers.Add(HeaderNames.IfNoneMatch, alone ? etag : $"\"tag1\", {etag}, \"tag2\"");

        using HttpResponseMessage response = await client.SendAsync(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotModified));
    }

    [Test]
    public async Task IfNoneMatch_304_When_Star()
    {
        using var host = await StaticFilesTestServer.Create();
        using var server = host.GetTestServer();
        using var client = server.CreateClient();

        using HttpResponseMessage original = await client.GetAsync(s_uri);

        using HttpRequestMessage request = new(HttpMethod.Get, s_uri);
        request.Headers.Add(HeaderNames.IfNoneMatch, "*");

        using HttpResponseMessage response = await client.SendAsync(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotModified));
    }

    [Test]
    public async Task IfModifiedSince_200_When_Earlier()
    {
        using var host = await StaticFilesTestServer.Create();
        using var server = host.GetTestServer();
        using var client = server.CreateClient();

        using HttpResponseMessage original = await client.GetAsync(s_uri);

        using HttpRequestMessage request = new(HttpMethod.Get, s_uri);
        request.Headers.Add(HeaderNames.IfModifiedSince,
            original.Content.Headers.LastModified!.Value.AddHours(-1).ToString("R"));

        using HttpResponseMessage response = await client.SendAsync(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task IfModifiedSince_304_When_Equal()
    {
        using var host = await StaticFilesTestServer.Create();
        using var server = host.GetTestServer();
        using var client = server.CreateClient();

        using HttpResponseMessage original = await client.GetAsync(s_uri);

        using HttpRequestMessage request = new(HttpMethod.Get, s_uri);
        request.Headers.Add(HeaderNames.IfModifiedSince,
            original.Content.Headers.LastModified!.Value.ToString("R"));

        using HttpResponseMessage response = await client.SendAsync(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotModified));
    }

    [Test]
    public async Task IfModifiedSince_304_When_Later()
    {
        using var host = await StaticFilesTestServer.Create();
        using var server = host.GetTestServer();
        using var client = server.CreateClient();

        using HttpResponseMessage original = await client.GetAsync(s_uri);

        using HttpRequestMessage request = new(HttpMethod.Get, s_uri);
        request.Headers.Add(HeaderNames.IfModifiedSince,
            original.Content.Headers.LastModified!.Value.AddHours(1).ToString("R"));

        using HttpResponseMessage response = await client.SendAsync(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotModified));
    }

    [Test]
    public async Task IfUnmodifiedSince_412_When_Earlier()
    {
        using var host = await StaticFilesTestServer.Create();
        using var server = host.GetTestServer();
        using var client = server.CreateClient();

        using HttpResponseMessage original = await client.GetAsync(s_uri);

        using HttpRequestMessage request = new(HttpMethod.Get, s_uri);
        request.Headers.Add(HeaderNames.IfUnmodifiedSince,
            original.Content.Headers.LastModified!.Value.AddHours(-1).ToString("R"));

        using HttpResponseMessage response = await client.SendAsync(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.PreconditionFailed));
    }

    [Test]
    public async Task IfUnmodifiedSince_200_When_Equal()
    {
        using var host = await StaticFilesTestServer.Create();
        using var server = host.GetTestServer();
        using var client = server.CreateClient();

        using HttpResponseMessage original = await client.GetAsync(s_uri);

        using HttpRequestMessage request = new(HttpMethod.Get, s_uri);
        request.Headers.Add(HeaderNames.IfUnmodifiedSince,
            original.Content.Headers.LastModified!.Value.ToString("R"));

        using HttpResponseMessage response = await client.SendAsync(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task IfUnmodifiedSince_200_When_Later()
    {
        using var host = await StaticFilesTestServer.Create();
        using var server = host.GetTestServer();
        using var client = server.CreateClient();

        using HttpResponseMessage original = await client.GetAsync(s_uri);

        using HttpRequestMessage request = new(HttpMethod.Get, s_uri);
        request.Headers.Add(HeaderNames.IfUnmodifiedSince,
            original.Content.Headers.LastModified!.Value.AddHours(1).ToString("R"));

        using HttpResponseMessage response = await client.SendAsync(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}
