using System.Net;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Net.Http.Headers;
using NUnit.Framework;

namespace MinimalApiBuilder.UnitTests.Middleware;

internal sealed class RangeTests
{
    private static readonly Uri s_rangeUri = new("/range.txt", UriKind.Relative);

    [Test]
    public async Task IfRange_With_Current_ETag_207()
    {
        using var host = await StaticFilesTestServer.Create();
        using var server = host.GetTestServer();
        using var client = server.CreateClient();

        using HttpResponseMessage original = await client.GetAsync(s_rangeUri);
        string foo = await original.Content.ReadAsStringAsync();

        Assert.That(original.Headers.ETag, Is.Not.Null);

        using HttpRequestMessage request = new(HttpMethod.Get, s_rangeUri);
        request.Headers.Add(HeaderNames.IfRange, original.Headers.ETag!.Tag);
        request.Headers.Add(HeaderNames.Range, "bytes=0-10");

        using HttpResponseMessage response = await client.SendAsync(request);

        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.PartialContent));
            Assert.That(response.Content.Headers.ContentLength, Is.EqualTo(11));
            Assert.That(response.Content.Headers.ContentRange!.ToString(), Is.EqualTo("bytes 0-10/62"));
            string content = await response.Content.ReadAsStringAsync();
            Assert.That(content, Is.EqualTo("0123456789a"));
        });
    }
}
