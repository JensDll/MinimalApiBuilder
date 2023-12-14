using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using MinimalApiBuilder.Middleware;
using MinimalApiBuilder.UnitTests.Infrastructure;
using NUnit.Framework;
#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#endif

namespace MinimalApiBuilder.UnitTests.Middleware;

internal sealed class CompressedStaticFileOptionsTests
{
    [Test]
    public void ContentEncoding_Order_Values_Must_Not_Be_Negative()
    {
        KeyValuePair<StringSegment, (int, string)>[] values =
        {
            new(ContentCodingNames.Br, (0, "br")),
            new(ContentCodingNames.Gzip, (-1, "gz"))
        };

        var exception = Assert.ThrowsAsync<OptionsValidationException>(async () =>
        {
            using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync(new CompressedStaticFileOptions
            {
#if NET8_0_OR_GREATER
                ContentEncoding = values.ToFrozenDictionary()
#else
                ContentEncoding = new Dictionary<StringSegment, (int, string)>(values)
#endif
            });
        });

        Assert.That(exception!.Message, Contains.Substring("values must not be negative"));
    }

    [Test]
    public void ContentEncoding_Keys_Must_Use_Case_Insensitive_Comparision()
    {
        KeyValuePair<StringSegment, (int, string)>[] values =
        {
            new(ContentCodingNames.Br, (0, "br")),
            new(ContentCodingNames.Gzip, (1, "gz"))
        };

        var exception = Assert.ThrowsAsync<OptionsValidationException>(async () =>
        {
            using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync(new CompressedStaticFileOptions
            {
#if NET8_0_OR_GREATER
                ContentEncoding = values.ToFrozenDictionary()
#else
                ContentEncoding = new Dictionary<StringSegment, (int, string)>(values)
#endif
            });
        });

        Assert.That(exception!.Message, Contains.Substring("keys must use case-insensitive ordinal comparison"));
    }
}
