using System.Net;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using MinimalApiBuilder.Middleware;
using MinimalApiBuilder.UnitTests.Infrastructure;
using NUnit.Framework;
#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#endif

namespace MinimalApiBuilder.UnitTests.Middleware;

internal sealed class AcceptEncodingTests
{
    private static readonly Uri s_uri = new("/data.txt", UriKind.Relative);

    [Test]
    public async Task Without_Quality_Chooses_Based_On_Configured_Order()
    {
        using HttpResponseMessage response = await MakeRequestAsync(new KeyValuePair<StringSegment, (int, string)>[]
        {
            new("br", (3, "br")),
            new("gzip", (2, "gz")),
            new("deflate", (1, "deflate"))
        }, "br, gzip, deflate");

        await AssertResponseAsync(response, "br");
    }

    [TestCase("br", "br")]
    [TestCase("gzip", "gz")]
    public async Task Unconfigured_Content_Encoding_Is_Ignored(string encoding, string extension)
    {
        using HttpResponseMessage response = await MakeRequestAsync(new KeyValuePair<StringSegment, (int, string)>[]
        {
            new(encoding, (0, extension))
        }, "br, gzip, deflate");

        await AssertResponseAsync(response, encoding);
    }

    [Test]
    public async Task Quality_Has_Higher_Precedence_Than_Order()
    {
        using HttpResponseMessage response = await MakeRequestAsync(new KeyValuePair<StringSegment, (int, string)>[]
        {
            new("br", (3, "br")),
            new("gzip", (2, "gz")),
            new("deflate", (1, "deflate"))
        }, "br;q=0.7, gzip;q=0.8, deflate;q=0.9");

        await AssertResponseAsync(response, "deflate");
    }

    [TestCase(2, 1, "gzip")]
    [TestCase(1, 2, "deflate")]
    public async Task Order_Decides_When_Quality_Is_The_Same(int gzipOrder, int deflateOrder, string encoding)
    {
        using HttpResponseMessage response = await MakeRequestAsync(new KeyValuePair<StringSegment, (int, string)>[]
        {
            new("br", (3, "br")),
            new("gzip", (gzipOrder, "gz")),
            new("deflate", (deflateOrder, "deflate"))
        }, "br;q=0.7, gzip;q=0.8, deflate;q=0.8");

        await AssertResponseAsync(response, encoding);
    }

    [TestCase(1, 2, 3, "deflate")]
    [TestCase(3, 2, 1, "br")]
    public async Task Default_Quality_Is_One(int brOrder, int gzipOrder, int deflateOrder, string encoding)
    {
        using HttpResponseMessage response = await MakeRequestAsync(new KeyValuePair<StringSegment, (int, string)>[]
        {
            new("br", (brOrder, "br")),
            new("gzip", (gzipOrder, "gz")),
            new("deflate", (deflateOrder, "deflate"))
        }, "br;q=1, gzip, deflate");

        await AssertResponseAsync(response, encoding);
    }

    [Test]
    public async Task Quality_Zero_Means_Not_Acceptable_And_Serves_Without_Content_Negotiation()
    {
        using HttpResponseMessage response = await MakeRequestAsync(new KeyValuePair<StringSegment, (int, string)>[]
        {
            new("br", (3, "br")),
            new("gzip", (2, "gz")),
            new("deflate", (1, "deflate"))
        }, "br;q=0, gzip;q=0, deflate;q=0");

        await AssertResponseAsync(response);
    }

    [Test]
    public async Task File_Without_Valid_Representation_Is_Served_Without_Content_Negotiation()
    {
        using HttpResponseMessage response = await MakeRequestAsync(new KeyValuePair<StringSegment, (int, string)>[]
        {
            new("zstd", (0, "zstd"))
        }, "br, gzip, deflate, zstd");

        await AssertResponseAsync(response);
    }

    private static async Task<HttpResponseMessage> MakeRequestAsync(
        IEnumerable<KeyValuePair<StringSegment, (int, string)>> contentEncoding,
        string acceptEncoding)
    {
        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync(new CompressedStaticFileOptions
        {
#if NET8_0_OR_GREATER
            ContentEncoding = contentEncoding.ToFrozenDictionary(StringSegmentComparer.OrdinalIgnoreCase)
#else
            ContentEncoding =
                new Dictionary<StringSegment, (int, string)>(contentEncoding, StringSegmentComparer.OrdinalIgnoreCase)
#endif
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
