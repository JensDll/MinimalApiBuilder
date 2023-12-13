using System.Collections.Frozen;
using System.Net;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using MinimalApiBuilder.Middleware;
using MinimalApiBuilder.UnitTests.Infrastructure;
using NUnit.Framework;

namespace MinimalApiBuilder.UnitTests.Middleware;

internal sealed class AcceptEncodingTests
{
    private static readonly Uri s_uri = new("/data.txt", UriKind.Relative);

    [Test]
    public async Task Without_Quality_Chooses_Based_On_Configured_Order()
    {
        using HttpResponseMessage response = await MakeRequestAsync(new KeyValuePair<StringSegment, int>[]
        {
            new("br", 3),
            new("gzip", 2),
            new("deflate", 1)
        }, "br, gzip, deflate");

        await AssertResponseAsync(response, ContentCodingNames.Br);
    }

    [TestCase("br")]
    [TestCase("gzip")]
    public async Task Unconfigured_Content_Encoding_Is_Ignored(string expectedEncoding)
    {
        using HttpResponseMessage response = await MakeRequestAsync(new KeyValuePair<StringSegment, int>[]
        {
            new(expectedEncoding, 0)
        }, "br, gzip, deflate");

        await AssertResponseAsync(response, expectedEncoding);
    }

    [Test]
    public async Task Quality_Has_Higher_Precedence_Than_Order()
    {
        using HttpResponseMessage response = await MakeRequestAsync(new KeyValuePair<StringSegment, int>[]
        {
            new("br", 3),
            new("gzip", 2),
            new("deflate", 1)
        }, "br;q=0.7, gzip;q=0.8, deflate;q=0.9");

        await AssertResponseAsync(response, "deflate");
    }

    [TestCase(2, 1, "gzip")]
    [TestCase(1, 2, "deflate")]
    public async Task Order_Decides_When_Quality_Is_The_Same(int gzipOrder, int deflateOrder, string expectedEncoding)
    {
        using HttpResponseMessage response = await MakeRequestAsync(new KeyValuePair<StringSegment, int>[]
        {
            new("br", 3),
            new("gzip", gzipOrder),
            new("deflate", deflateOrder)
        }, "br;q=0.7, gzip;q=0.8, deflate;q=0.8");

        await AssertResponseAsync(response, expectedEncoding);
    }

    [TestCase(1, 2, 3, "deflate")]
    [TestCase(3, 2, 1, "br")]
    public async Task Default_Quality_Is_One(int brOrder, int gzipOrder, int deflateOrder, string expectedEncoding)
    {
        using HttpResponseMessage response = await MakeRequestAsync(new KeyValuePair<StringSegment, int>[]
        {
            new("br", brOrder),
            new("gzip", gzipOrder),
            new("deflate", deflateOrder)
        }, "br;q=1, gzip, deflate");

        await AssertResponseAsync(response, expectedEncoding);
    }

    [Test]
    public async Task Quality_Zero_Means_Not_Acceptable_And_Serves_Without_Content_Negotiation()
    {
        using HttpResponseMessage response = await MakeRequestAsync(new KeyValuePair<StringSegment, int>[]
        {
            new("br", 3),
            new("gzip", 2),
            new("deflate", 1)
        }, "br;q=0, gzip;q=0, deflate;q=0");

        await AssertResponseAsync(response);
    }

    private static async Task<HttpResponseMessage> MakeRequestAsync(
        IEnumerable<KeyValuePair<StringSegment, int>> contentEncodingOrder,
        string acceptEncoding)
    {
        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync(new CompressedStaticFileOptions
        {
            ContentEncodingOrder = contentEncodingOrder.ToFrozenDictionary()
        });

        using HttpRequestMessage request = new(HttpMethod.Get, s_uri);
        request.Headers.Add(HeaderNames.AcceptEncoding, acceptEncoding);

        return await server.Client.SendAsync(request);
    }

    private static async Task AssertResponseAsync(HttpResponseMessage response, string expectedEncoding)
    {
        ICollection<string> contentEncoding = response.Content.Headers.ContentEncoding;
        string content = await response.Content.ReadAsStringAsync();

        Assert.That(contentEncoding, Has.Count.EqualTo(1));

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(contentEncoding.Single(), Is.EqualTo(expectedEncoding));
            Assert.That(content, Is.EqualTo($"{expectedEncoding} data"));
        });
    }

    private static async Task AssertResponseAsync(HttpResponseMessage response)
    {
        string content = await response.Content.ReadAsStringAsync();

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content.Headers.ContentEncoding, Is.Empty);
            Assert.That(content, Is.EqualTo("data"));
        });
    }
}
