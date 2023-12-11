using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Primitives;
#if NET8_0_OR_GREATER
using System.Collections.Frozen;

#else
using System.Collections.ObjectModel;
#endif

namespace MinimalApiBuilder.Middleware;

/// <summary>
/// Options for server static files.
/// </summary>
public class CompressedStaticFileOptions : StaticFileOptions
{
    private static readonly KeyValuePair<StringSegment, int>[] s_defaultContentEncodingOrder =
    {
        new("br", 1),
        new("gzip", 0)
    };

#if NET8_0_OR_GREATER
    /// <summary>
    /// Preferred content encoding order based on the Accept-Encoding header field.
    /// </summary>
#pragma warning disable CA2227 False positive
    public FrozenDictionary<StringSegment, int> ContentEncodingOrder { get; set; } =
#pragma warning restore CA2227
        s_defaultContentEncodingOrder.ToFrozenDictionary(StringSegmentComparer.OrdinalIgnoreCase);
#else
    /// <summary>
    /// Preferred content encoding order based on the Accept-Encoding header field.
    /// </summary>
    public ReadOnlyDictionary<StringSegment, int> ContentEncodingOrder { get; set; } =
        new Dictionary<StringSegment, int>(s_defaultContentEncodingOrder, StringSegmentComparer.OrdinalIgnoreCase)
            .AsReadOnly();
#endif
}
