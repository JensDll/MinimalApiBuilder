using System.Collections.Frozen;
using System.Net;
using Microsoft.AspNetCore.TestHost;
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

        await AssertResponseAsync(response, "br");
    }

    [Test]
    public async Task Unconfigured_Content_Encoding_Is_Ignored()
    {
        using HttpResponseMessage responseA = await MakeRequestAsync(new KeyValuePair<StringSegment, int>[]
        {
            new("br", 0)
        }, "br, gzip, deflate");

        using HttpResponseMessage responseB = await MakeRequestAsync(new KeyValuePair<StringSegment, int>[]
        {
            new("gzip", 0)
        }, "br, gzip, deflate");

        await Assert.MultipleAsync(() => Task.WhenAll(
            AssertResponseAsync(responseA, "br"),
            AssertResponseAsync(responseB, "gzip")));
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

    [Test]
    public async Task Order_Decides_When_Quality_Is_The_Same()
    {
        using HttpResponseMessage response = await MakeRequestAsync(new KeyValuePair<StringSegment, int>[]
        {
            new("br", 3),
            new("gzip", 2),
            new("deflate", 1)
        }, "br;q=0.7, gzip;q=0.8, deflate;q=0.8");

        await AssertResponseAsync(response, "gzip");
    }

    [Test]
    public async Task Default_Quality_Is_One()
    {
        using HttpResponseMessage responseA = await MakeRequestAsync(new KeyValuePair<StringSegment, int>[]
        {
            new("br", 1),
            new("gzip", 2),
            new("deflate", 3)
        }, "br;q=1, gzip, deflate");

        using HttpResponseMessage responseB = await MakeRequestAsync(new KeyValuePair<StringSegment, int>[]
        {
            new("br", 3),
            new("gzip", 2),
            new("deflate", 1)
        }, "br;q=1, gzip, deflate");

        await Assert.MultipleAsync(() => Task.WhenAll(
            AssertResponseAsync(responseA, "deflate"),
            AssertResponseAsync(responseB, "br")));
    }

    private static async Task<HttpResponseMessage> MakeRequestAsync(
        IEnumerable<KeyValuePair<StringSegment, int>> contentEncodingOrder,
        string acceptEncoding)
    {
        using var host = await StaticFilesTestServer.Create(new CompressedStaticFileOptions
        {
            ContentEncodingOrder = contentEncodingOrder.ToFrozenDictionary()
        });
        using var server = host.GetTestServer();
        using var client = server.CreateClient();

        using HttpRequestMessage request = new(HttpMethod.Get, s_uri);
        request.Headers.Add(HeaderNames.AcceptEncoding, acceptEncoding);

        return await client.SendAsync(request);
    }

    private static async Task AssertResponseAsync(HttpResponseMessage response, string expectedEncoding)
    {
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        ICollection<string> contentEncoding = response.Content.Headers.ContentEncoding;
        Assert.That(contentEncoding, Has.Count.EqualTo(1));
        Assert.That(contentEncoding.Single(), Is.EqualTo(expectedEncoding));
        string content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Is.EqualTo($"{expectedEncoding} data"));
    }
}
