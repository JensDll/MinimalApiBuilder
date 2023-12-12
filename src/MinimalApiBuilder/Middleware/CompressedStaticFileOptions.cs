#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#else
using System.Collections.ObjectModel;
#endif
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Primitives;

namespace MinimalApiBuilder.Middleware;

// False positive. Frozen collections need to be excluded from the rule, see:
// https://github.com/Evangelink/roslyn-analyzers/blob/master/src/NetAnalyzers/Core/Microsoft.CodeQuality.Analyzers/ApiDesignGuidelines/CollectionPropertiesShouldBeReadOnly.cs
#pragma warning disable CA2227

/// <summary>
/// Options for serving static pre-compressed files.
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
    /// The preferred <a href="https://www.rfc-editor.org/rfc/rfc9110.html#section-12.5.3">Accept-Encoding</a>
    /// order when the best match cannot be determined by the quality value in the header field.
    /// </summary>
    /// <remarks>
    /// <see cref="CompressedStaticFileMiddleware" /> will only serve content-coded representations
    /// with names listed in this dictionary.
    /// </remarks>
    public FrozenDictionary<StringSegment, int> ContentEncodingOrder { get; set; } =
        s_defaultContentEncodingOrder.ToFrozenDictionary(StringSegmentComparer.OrdinalIgnoreCase);
#else
    /// <summary>
    /// The preferred <a href="https://www.rfc-editor.org/rfc/rfc9110.html#section-12.5.3">Accept-Encoding</a>
    /// order when the best match cannot be determined by the quality value in the header field.
    /// </summary>
    /// <remarks>
    /// <see cref="CompressedStaticFileMiddleware" /> will only serve content-coded representations
    /// with names listed in this dictionary.
    /// </remarks>
    public ReadOnlyDictionary<StringSegment, int> ContentEncodingOrder { get; set; } =
        new Dictionary<StringSegment, int>(s_defaultContentEncodingOrder, StringSegmentComparer.OrdinalIgnoreCase)
            .AsReadOnly();
#endif
}
