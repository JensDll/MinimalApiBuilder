using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Primitives;
#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#endif

namespace MinimalApiBuilder.Middleware;

#pragma warning disable CA2227

/// <summary>
/// Options for serving static pre-compressed files.
/// </summary>
public class CompressedStaticFileOptions : StaticFileOptions
{
    private static readonly KeyValuePair<StringSegment, (int, string)>[] s_defaultContentEncodingOrder =
    {
        new(ContentCodingNames.Br, (1, "br")),
        new(ContentCodingNames.Gzip, (0, "gz"))
    };

#if NET8_0_OR_GREATER
    /// <summary>
    /// The available pre-compressed file formats. Keys are <see cref="ContentCodingNames" />, and
    /// values are tuples of order and file extension. <see cref="CompressedStaticFileMiddleware" />
    /// uses order to prioritize the selected representation when the quality values of
    /// <a href="https://www.rfc-editor.org/rfc/rfc9110.html#section-12.5.3">Accept-Encoding</a> are equal.
    /// </summary>
    public FrozenDictionary<StringSegment, (int, string)> ContentEncoding { get; set; } =
        s_defaultContentEncodingOrder.ToFrozenDictionary(StringSegmentComparer.OrdinalIgnoreCase);
#else
    /// <summary>
    /// The available pre-compressed file formats. Keys are <see cref="ContentCodingNames" />, and
    /// values are tuples of order and file extension. <see cref="CompressedStaticFileMiddleware" />
    /// uses order to prioritize the selected representation when the quality values of
    /// <a href="https://www.rfc-editor.org/rfc/rfc9110.html#section-12.5.3">Accept-Encoding</a> are equal.
    /// </summary>
    public Dictionary<StringSegment, (int, string)> ContentEncoding { get; set; } =
        new(s_defaultContentEncodingOrder, StringSegmentComparer.OrdinalIgnoreCase);
#endif
}
