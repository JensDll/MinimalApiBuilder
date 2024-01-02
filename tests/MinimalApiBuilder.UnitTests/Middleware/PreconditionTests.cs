using System.Net;
using Microsoft.Net.Http.Headers;
using MinimalApiBuilder.UnitTests.Infrastructure;
using NUnit.Framework;

namespace MinimalApiBuilder.UnitTests.Middleware;

internal sealed class PreconditionTests
{
    [Test]
    public async Task IfMatch_412_When_Not_Listed()
    {
        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync();

        using HttpRequestMessage request = new(HttpMethod.Get, StaticUri.DataTxtUri);
        request.Headers.Add(HeaderNames.IfMatch, "\"not-matching\"");

        using HttpResponseMessage response = await server.Client.SendAsync(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.PreconditionFailed));
    }

    [Test]
    public async Task IfMatch_200_When_Listed([Values] bool alone)
    {
        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync();

        using HttpResponseMessage original = await server.Client.GetAsync(StaticUri.DataTxtUri);
        string etag = original.Headers.ETag!.Tag;

        using HttpRequestMessage request = new(HttpMethod.Get, StaticUri.DataTxtUri);
        request.Headers.Add(HeaderNames.IfMatch, alone ? etag : $"\"tag1\", {etag}, \"tag2\"");

        using HttpResponseMessage response = await server.Client.SendAsync(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task IfMatch_200_When_Star()
    {
        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync();

        using HttpResponseMessage original = await server.Client.GetAsync(StaticUri.DataTxtUri);

        using HttpRequestMessage request = new(HttpMethod.Get, StaticUri.DataTxtUri);
        request.Headers.Add(HeaderNames.IfMatch, "*");

        using HttpResponseMessage response = await server.Client.SendAsync(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task IfNoneMatch_200_When_Not_Listed()
    {
        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync();

        using HttpRequestMessage request = new(HttpMethod.Get, StaticUri.DataTxtUri);
        request.Headers.Add(HeaderNames.IfNoneMatch, "\"not-matching\"");

        using HttpResponseMessage response = await server.Client.SendAsync(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task IfNoneMatch_304_When_Listed([Values] bool alone, [Values] bool weak)
    {
        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync();

        using HttpResponseMessage original = await server.Client.GetAsync(StaticUri.DataTxtUri);
        string etag = weak ? $"W/{original.Headers.ETag!.Tag}" : original.Headers.ETag!.Tag;

        Assert.That(original.Headers.ETag.IsWeak, Is.False);

        using HttpRequestMessage request = new(HttpMethod.Get, StaticUri.DataTxtUri);
        request.Headers.Add(HeaderNames.IfNoneMatch, alone ? etag : $"\"tag1\", {etag}, \"tag2\"");

        using HttpResponseMessage response = await server.Client.SendAsync(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotModified));
    }

    [Test]
    public async Task IfNoneMatch_304_When_Star()
    {
        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync();

        using HttpRequestMessage request = new(HttpMethod.Get, StaticUri.DataTxtUri);
        request.Headers.Add(HeaderNames.IfNoneMatch, "*");

        using HttpResponseMessage response = await server.Client.SendAsync(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotModified));
    }

    [Test]
    public async Task IfModifiedSince_200_When_Earlier()
    {
        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync();

        using HttpResponseMessage original = await server.Client.GetAsync(StaticUri.DataTxtUri);

        using HttpRequestMessage request = new(HttpMethod.Get, StaticUri.DataTxtUri);
        request.Headers.Add(HeaderNames.IfModifiedSince,
            original.Content.Headers.LastModified!.Value.AddHours(-1).ToString("R"));

        using HttpResponseMessage response = await server.Client.SendAsync(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task IfModifiedSince_304_When_Equal()
    {
        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync();

        using HttpResponseMessage original = await server.Client.GetAsync(StaticUri.DataTxtUri);

        using HttpRequestMessage request = new(HttpMethod.Get, StaticUri.DataTxtUri);
        request.Headers.Add(HeaderNames.IfModifiedSince,
            original.Content.Headers.LastModified!.Value.ToString("R"));

        using HttpResponseMessage response = await server.Client.SendAsync(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotModified));
    }

    [Test]
    public async Task IfModifiedSince_304_When_Later()
    {
        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync();

        using HttpResponseMessage original = await server.Client.GetAsync(StaticUri.DataTxtUri);

        using HttpRequestMessage request = new(HttpMethod.Get, StaticUri.DataTxtUri);
        request.Headers.Add(HeaderNames.IfModifiedSince,
            original.Content.Headers.LastModified!.Value.AddHours(1).ToString("R"));

        using HttpResponseMessage response = await server.Client.SendAsync(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotModified));
    }

    [Test]
    public async Task IfUnmodifiedSince_412_When_Earlier()
    {
        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync();

        using HttpResponseMessage original = await server.Client.GetAsync(StaticUri.DataTxtUri);

        using HttpRequestMessage request = new(HttpMethod.Get, StaticUri.DataTxtUri);
        request.Headers.Add(HeaderNames.IfUnmodifiedSince,
            original.Content.Headers.LastModified!.Value.AddHours(-1).ToString("R"));

        using HttpResponseMessage response = await server.Client.SendAsync(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.PreconditionFailed));
    }

    [Test]
    public async Task IfUnmodifiedSince_200_When_Equal()
    {
        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync();

        using HttpResponseMessage original = await server.Client.GetAsync(StaticUri.DataTxtUri);

        using HttpRequestMessage request = new(HttpMethod.Get, StaticUri.DataTxtUri);
        request.Headers.Add(HeaderNames.IfUnmodifiedSince,
            original.Content.Headers.LastModified!.Value.ToString("R"));

        using HttpResponseMessage response = await server.Client.SendAsync(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task IfUnmodifiedSince_200_When_Later()
    {
        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync();

        using HttpResponseMessage original = await server.Client.GetAsync(StaticUri.DataTxtUri);

        using HttpRequestMessage request = new(HttpMethod.Get, StaticUri.DataTxtUri);
        request.Headers.Add(HeaderNames.IfUnmodifiedSince,
            original.Content.Headers.LastModified!.Value.AddHours(1).ToString("R"));

        using HttpResponseMessage response = await server.Client.SendAsync(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}
