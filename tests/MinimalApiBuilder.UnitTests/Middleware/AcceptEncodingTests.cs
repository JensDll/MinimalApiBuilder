using System.Net;
using Microsoft.Net.Http.Headers;
using MinimalApiBuilder.Middleware;
using MinimalApiBuilder.UnitTests.Infrastructure;
using NUnit.Framework;
#if NET8_0_OR_GREATER
#endif

namespace MinimalApiBuilder.UnitTests.Middleware;

internal sealed class AcceptEncodingTests
{
    private static readonly Uri s_dataUri = new("/data.txt", UriKind.Relative);
    private static readonly Uri s_rangeUri = new("/range.txt", UriKind.Relative);
    private static readonly Uri s_fooUri = new("/foo.txt", UriKind.Relative);

    [Test]
    public async Task Without_Quality_Chooses_Based_On_Configured_Order()
    {
        using HttpResponseMessage response = await MakeRequestAsync(new KeyValuePair<string, (int, string)>[]
        {
            new("br", (3, "br")),
            new("gzip", (2, "gz")),
            new("deflate", (1, "deflate"))
        }, "br, gzip, deflate");

        await AssertResponseAsync(response, "br");
    }

    [TestCase("br", "br")]
    [TestCase("gzip", "gz")]
    public async Task Unconfigured_Content_Coding_Is_Ignored(string contentCoding, string extension)
    {
        using HttpResponseMessage response = await MakeRequestAsync(new KeyValuePair<string, (int, string)>[]
        {
            new(contentCoding, (0, extension))
        }, "br, gzip, deflate");

        await AssertResponseAsync(response, contentCoding);
    }

    [Test]
    public async Task Quality_Has_Higher_Precedence_Than_Order()
    {
        using HttpResponseMessage response = await MakeRequestAsync(new KeyValuePair<string, (int, string)>[]
        {
            new("br", (3, "br")),
            new("gzip", (2, "gz")),
            new("deflate", (1, "deflate"))
        }, "br;q=0.7, gzip;q=0.8, deflate;q=0.9");

        await AssertResponseAsync(response, "deflate");
    }

    // Both at the end
    [TestCase(2, 1, "br;q=0.7, gzip;q=0.8, deflate;q=0.8", "gzip")]
    [TestCase(1, 2, "br;q=0.7, gzip;q=0.8, deflate;q=0.8", "deflate")]
    // Both at the beginning
    [TestCase(2, 1, "gzip;q=0.8, deflate;q=0.8, br;q=0.7", "gzip")]
    [TestCase(1, 2, "gzip;q=0.8, deflate;q=0.8, br;q=0.7", "deflate")]
    // One left, one right
    [TestCase(2, 1, "gzip;q=0.8, br;q=0.7, deflate;q=0.8", "gzip")]
    [TestCase(1, 2, "gzip;q=0.8, br;q=0.7, deflate;q=0.8", "deflate")]
    public async Task Order_Decides_When_Quality_Is_The_Same(int gzipOrder, int deflateOrder,
        string acceptEncoding, string expectedContentCoding)
    {
        using HttpResponseMessage response = await MakeRequestAsync(new KeyValuePair<string, (int, string)>[]
        {
            new("br", (3, "br")),
            new("gzip", (gzipOrder, "gz")),
            new("deflate", (deflateOrder, "deflate"))
        }, acceptEncoding);

        await AssertResponseAsync(response, expectedContentCoding);
    }

    [Test]
    public async Task Quality_Zero_Means_Not_Acceptable_And_Serves_Without_Content_Encoding()
    {
        using HttpResponseMessage response = await MakeRequestAsync(new KeyValuePair<string, (int, string)>[]
        {
            new("br", (3, "br")),
            new("gzip", (2, "gz")),
            new("deflate", (1, "deflate"))
        }, "br;q=0, gzip;q=0, deflate;q=0");

        await AssertResponseAsync(response);
    }

    [Test]
    public async Task File_Without_Valid_Representation_Serves_Without_Content_Encoding()
    {
        using HttpResponseMessage response = await MakeRequestAsync(new KeyValuePair<string, (int, string)>[]
        {
            new("zstd", (0, "zstd"))
        }, "br, gzip, deflate, zstd");

        await AssertResponseAsync(response);
    }

    [Test]
    public async Task Empty_Accept_Encoding_Serves_Without_Content_Encoding()
    {
        using HttpResponseMessage response = await MakeRequestAsync(new KeyValuePair<string, (int, string)>[]
        {
            new("br", (3, "br")),
            new("gzip", (2, "gz")),
            new("deflate", (1, "deflate"))
        }, string.Empty);

        await AssertResponseAsync(response);
    }

    [TestCase(3, 2, 1, "*", "br")]
    [TestCase(2, 3, 1, "*", "gzip")]
    [TestCase(1, 2, 3, "*", "deflate")]
    [TestCase(3, 2, 1, "deflate;q=0.4, *;q=0.5", "br")]
    [TestCase(1, 2, 3, "deflate;q=0.4, *;q=0.5", "gzip")]
    [TestCase(3, 2, 1, "*;q=0.5, deflate", "deflate")]
    [TestCase(3, 2, 1, "*;q=0.5, deflate;q=0.4", "br")]
    [TestCase(2, 3, 1, "*;q=0.5, deflate;q=0.4", "gzip")]
    [TestCase(3, 2, 1, "*;q=0.5, br;q=0.4", "gzip")]
    [TestCase(3, 2, 1, "*;q=0.5, br;q=0.4, gzip;q=0.4, deflate;q=0.4", "br")]
    [TestCase(3, 2, 1, "*;q=0.5, deflate;q=0.5", "deflate")]
    [TestCase(3, 2, 1, "deflate;q=0.5, *;q=0.5", "deflate")]
    [TestCase(3, 2, 1, "br;q=0.5, *;q=0.5", "br")]
    public async Task Wildcard_Chooses_The_Best_Available_Content_Coding(int brOrder, int gzipOrder,
        int deflateOrder, string acceptEncoding, string expectedContentCoding)
    {
        using HttpResponseMessage response = await MakeRequestAsync(new KeyValuePair<string, (int, string)>[]
        {
            new("br", (brOrder, "br")),
            new("gzip", (gzipOrder, "gz")),
            new("deflate", (deflateOrder, "deflate"))
        }, acceptEncoding);

        await AssertResponseAsync(response, expectedContentCoding);
    }

    private static readonly string[] s_expectedAcceptEncoding = ["br", "gzip", "deflate"];

    [TestCase("br, identity;q=0")]
    [TestCase("br, *;q=0")]
    public async Task UnsupportedMediaType_415_When_Identity_Forbidden_And_Compressed_File_Not_Available(
        string acceptEncoding)
    {
        using HttpResponseMessage response = await MakeRequestAsync(new KeyValuePair<string, (int, string)>[]
        {
            new("br", (3, "br")),
            new("gzip", (2, "gz")),
            new("deflate", (1, "deflate"))
        }, acceptEncoding, s_rangeUri);

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.UnsupportedMediaType));
            Assert.That(response.Headers.TryGetValues(HeaderNames.AcceptEncoding, out IEnumerable<string>? values),
                Is.True);
            Assert.That(values, Is.EqualTo(s_expectedAcceptEncoding));
        });
    }

    [TestCase("br, identity;q=0, identity;q=0.1")]
    [TestCase("identity;q=0.1, identity;q=0, br")]
    [TestCase("br, *;q=0, identity;q=0.1")]
    [TestCase("br, identity;q=0.1, *;q=0")]
    public async Task Identity_Forbidden_Ignored_With_More_Specific_Entry(
        string acceptEncoding)
    {
        using HttpResponseMessage response = await MakeRequestAsync(new KeyValuePair<string, (int, string)>[]
        {
            new("br", (3, "br")),
            new("gzip", (2, "gz")),
            new("deflate", (1, "deflate"))
        }, acceptEncoding, s_rangeUri);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task Identity_Serves_Without_Content_Encoding_When_Available()
    {
        using HttpResponseMessage response = await MakeRequestAsync(new KeyValuePair<string, (int, string)>[]
        {
            new("br", (1, "br")),
            new("gzip", (0, "gz"))
        }, "identity, br;q=0.9");

        await AssertResponseAsync(response);
    }

    [Test]
    public async Task Identity_Serves_With_Content_Encoding_When_Not_Available()
    {
        using HttpResponseMessage response = await MakeRequestAsync(new KeyValuePair<string, (int, string)>[]
        {
            new("br", (1, "br")),
            new("gzip", (0, "gz"))
        }, "identity, br;q=0.9", s_fooUri);

        await AssertResponseAsync(response, "br");
    }

    private static Task<HttpResponseMessage> MakeRequestAsync(
        IReadOnlyList<KeyValuePair<string, (int, string)>> contentEncoding,
        string acceptEncoding)
    {
        return MakeRequestAsync(contentEncoding, acceptEncoding, s_dataUri);
    }

    private static async Task<HttpResponseMessage> MakeRequestAsync(
        IReadOnlyList<KeyValuePair<string, (int, string)>> contentEncoding,
        string acceptEncoding,
        Uri uri)
    {
        using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync(new CompressedStaticFileOptions
        {
            ContentCoding = contentEncoding
        });

        using HttpRequestMessage request = new(HttpMethod.Get, uri);
        request.Headers.Add(HeaderNames.AcceptEncoding, acceptEncoding);

        return await server.Client.SendAsync(request);
    }

    private static async Task AssertResponseAsync(HttpResponseMessage response, string expectedContentCoding)
    {
        ICollection<string> contentEncoding = response.Content.Headers.ContentEncoding;
        string content = await response.Content.ReadAsStringAsync();

        Assert.That(contentEncoding, Has.Count.EqualTo(1));

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(contentEncoding.Single(), Is.EqualTo(expectedContentCoding));
            Assert.That(content, Is.EqualTo($"{expectedContentCoding} data"));
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
