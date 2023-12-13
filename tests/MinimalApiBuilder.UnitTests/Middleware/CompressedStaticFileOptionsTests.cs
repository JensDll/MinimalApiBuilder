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
    public void ContentEncodingOrder_Values_Must_Not_Be_Negative()
    {
        KeyValuePair<StringSegment, int>[] values =
        [
            new KeyValuePair<StringSegment, int>(ContentCodingNames.Br, 0),
            new KeyValuePair<StringSegment, int>(ContentCodingNames.Gzip, -1)
        ];

        var exception = Assert.ThrowsAsync<OptionsValidationException>(async () =>
        {
            using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync(new CompressedStaticFileOptions
            {
#if NET8_0_OR_GREATER
                ContentEncodingOrder = values.ToFrozenDictionary()
#else
                ContentEncodingOrder = new Dictionary<StringSegment, int>(values)
#endif
            });
        });

        Assert.That(exception!.Message, Contains.Substring("values must not be negative"));
    }

    [Test]
    public void ContentEncodingOrder_Keys_Must_Use_Case_Insensitive_Comparision()
    {
        KeyValuePair<StringSegment, int>[] values =
        [
            new KeyValuePair<StringSegment, int>(ContentCodingNames.Br, 1),
            new KeyValuePair<StringSegment, int>(ContentCodingNames.Gzip, 2)
        ];

        var exception = Assert.ThrowsAsync<OptionsValidationException>(async () =>
        {
            using StaticFilesTestServer server = await StaticFilesTestServer.CreateAsync(new CompressedStaticFileOptions
            {
#if NET8_0_OR_GREATER
                ContentEncodingOrder = values.ToFrozenDictionary()
#else
                ContentEncodingOrder = new Dictionary<StringSegment, int>(values)
#endif
            });
        });

        Assert.That(exception!.Message, Contains.Substring("keys must use case-insensitive ordinal comparison"));
    }
}
