using System.Net;
using Microsoft.Net.Http.Headers;
using MinimalApiBuilder.UnitTests.Infrastructure;
using NUnit.Framework;
using ContentRangeHeaderValue = System.Net.Http.Headers.ContentRangeHeaderValue;
using EntityTagHeaderValue = System.Net.Http.Headers.EntityTagHeaderValue;

namespace MinimalApiBuilder.UnitTests.Middleware;

internal sealed class RangeTests
{
    [Test]
    public async Task IfRange_207_With_Current_ETag()
    {
        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync();

        using HttpResponseMessage original = await server.Client.GetAsync(StaticUri.RangeTxtUri);
        EntityTagHeaderValue originalEtag = original.Headers.ETag!;

        using HttpRequestMessage request = new(HttpMethod.Get, StaticUri.RangeTxtUri);
        request.Headers.Add(HeaderNames.IfRange, originalEtag.Tag);
        request.Headers.Add(HeaderNames.Range, "bytes=0-10");

        using HttpResponseMessage response = await server.Client.SendAsync(request);

        await Assert207Async(response, new ContentRangeHeaderValue(0, 10, 62), "0123456789a");
    }

    [Test]
    public async Task IfRange_207_With_Current_Date()
    {
        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync();

        using HttpResponseMessage original = await server.Client.GetAsync(StaticUri.RangeTxtUri);

        using HttpRequestMessage request = new(HttpMethod.Get, StaticUri.RangeTxtUri);
        request.Headers.Add(HeaderNames.IfRange, original.Content.Headers.LastModified!.Value.ToString("R"));
        request.Headers.Add(HeaderNames.Range, "bytes=0-10");

        using HttpResponseMessage response = await server.Client.SendAsync(request);

        await Assert207Async(response, new ContentRangeHeaderValue(0, 10, 62), "0123456789a");
    }

    [Test]
    public async Task IfRange_200_With_Outdated_ETag()
    {
        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync();

        using HttpResponseMessage original = await server.Client.GetAsync(StaticUri.RangeTxtUri);

        using HttpRequestMessage request = new(HttpMethod.Get, StaticUri.RangeTxtUri);
        request.Headers.Add(HeaderNames.IfRange, "\"outdated\"");
        request.Headers.Add(HeaderNames.Range, "bytes=0-10");

        using HttpResponseMessage response = await server.Client.SendAsync(request);

        await Assert200Async(response, await original.Content.ReadAsStringAsync());
    }

    [Test]
    public async Task IfRange_200_With_Unequal_Date([Values(-1, 1)] double hours)
    {
        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync();

        using HttpResponseMessage original = await server.Client.GetAsync(StaticUri.RangeTxtUri);

        using HttpRequestMessage request = new(HttpMethod.Get, StaticUri.RangeTxtUri);
        request.Headers.Add(HeaderNames.IfRange,
            original.Content.Headers.LastModified!.Value.AddHours(hours).ToString("R"));
        request.Headers.Add(HeaderNames.Range, "bytes=0-10");

        using HttpResponseMessage response = await server.Client.SendAsync(request);

        await Assert200Async(response, await original.Content.ReadAsStringAsync());
    }

    [Test]
    public async Task IfRange_Ignored_Without_Range()
    {
        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync();

        using HttpResponseMessage original = await server.Client.GetAsync(StaticUri.RangeTxtUri);
        EntityTagHeaderValue originalEtag = original.Headers.ETag!;

        using HttpRequestMessage request = new(HttpMethod.Get, StaticUri.RangeTxtUri);
        request.Headers.Add(HeaderNames.IfRange, originalEtag.Tag);

        using HttpResponseMessage response = await server.Client.SendAsync(request);

        await Assert200Async(response, await original.Content.ReadAsStringAsync());
    }

    [TestCase("bytes=63-120")] // int-range, first-pos greater than length
    [TestCase("bytes=63-")] // int-range, first-pos greater than length, without last-pos
    [TestCase("bytes=62-100")] // int-range, firs-pos equals length
    [TestCase("bytes=62-")] // int-range, first-pos equals length, without last-pos
    [TestCase("bytes=-0")] // suffix-range, zero suffix-length
    public async Task Range_Not_Satisfiable(string rangeHeader)
    {
        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync();

        using HttpRequestMessage request = new(HttpMethod.Get, StaticUri.RangeTxtUri);
        request.Headers.Add(HeaderNames.Range, rangeHeader);

        using HttpResponseMessage response = await server.Client.SendAsync(request);

        await Assert416Async(response, 62);
    }

    [TestCase("bytes=0-0", 0, 0, "0")]
    [TestCase("bytes=0-9", 0, 9, "0123456789")]
    [TestCase("bytes=10-35", 10, 35, "abcdefghijklmnopqrstuvwxyz")]
    [TestCase("bytes=36-61", 36, 61, "ABCDEFGHIJKLMNOPQRSTUVWXYZ")]
    [TestCase("bytes=36-", 36, 61, "ABCDEFGHIJKLMNOPQRSTUVWXYZ")]
    [TestCase("bytes=-26", 36, 61, "ABCDEFGHIJKLMNOPQRSTUVWXYZ")]
    [TestCase("bytes=0-", 0, 61, "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ")]
    [TestCase("bytes=-100", 0, 61, "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ")]
    public async Task Serves_Partial_Content(string rangeHeader, long expectedStart, long expectedEnd,
        string expectedContent)
    {
        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync();

        using HttpRequestMessage request = new(HttpMethod.Get, StaticUri.RangeTxtUri);
        request.Headers.Add(HeaderNames.Range, rangeHeader);

        using HttpResponseMessage response = await server.Client.SendAsync(request);

        await Assert207Async(response, new ContentRangeHeaderValue(expectedStart, expectedEnd, 62), expectedContent);
    }

    private static Task Assert200Async(
        HttpResponseMessage response,
        string expectedContent)
    {
        return Assert.MultipleAsync(async () =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content.Headers.ContentLength, Is.EqualTo(expectedContent.Length));
            string content = await response.Content.ReadAsStringAsync();
            Assert.That(content, Is.EqualTo(expectedContent));
        });
    }

    private static Task Assert207Async(
        HttpResponseMessage response,
        ContentRangeHeaderValue expectedContentRange,
        string expectedContent)
    {
        return Assert.MultipleAsync(async () =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.PartialContent));
            Assert.That(response.Content.Headers.ContentLength, Is.EqualTo(expectedContent.Length));
            Assert.That(response.Content.Headers.ContentRange, Is.EqualTo(expectedContentRange));
            string content = await response.Content.ReadAsStringAsync();
            Assert.That(content, Is.EqualTo(expectedContent));
        });
    }

    private static Task Assert416Async(HttpResponseMessage response, int length)
    {
        return Assert.MultipleAsync(async () =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.RequestedRangeNotSatisfiable));
            Assert.That(response.Content.Headers.ContentRange, Is.EqualTo(new ContentRangeHeaderValue(length)));
            string content = await response.Content.ReadAsStringAsync();
            Assert.That(content, Is.EqualTo(string.Empty));
        });
    }
}
