using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Primitives;
#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#endif

namespace MinimalApiBuilder.Middleware;

/// <summary>
/// Options for serving static pre-compressed files.
/// </summary>
public class CompressedStaticFileOptions : StaticFileOptions
{
    /// <summary>
    /// The available pre-compressed file formats. Keys are <see cref="ContentCodingNames" />, and
    /// values are tuples of order and file extension. <see cref="CompressedStaticFileMiddleware" />
    /// uses order to prioritize the selected representation when the quality values of
    /// <a href="https://www.rfc-editor.org/rfc/rfc9110.html#section-12.5.3">Accept-Encoding</a> are equal.
    /// </summary>
    public IReadOnlyList<KeyValuePair<string, (int Order, string Extension)>> ContentCoding { get; set; } =
        new KeyValuePair<string, (int, string)>[]
        {
            new(ContentCodingNames.Br, (1, "br")),
            new(ContentCodingNames.Gzip, (0, "gz"))
        };

    /// <inheritdoc cref="StaticFileOptions.OnPrepareResponse" />
    public new Action<CompressedStaticFileResponseContext> OnPrepareResponse { get; set; } = static _ => { };

#if NET8_0_OR_GREATER
    /// <inheritdoc cref="StaticFileOptions.OnPrepareResponseAsync" />
    public new Func<CompressedStaticFileResponseContext, Task> OnPrepareResponseAsync { get; set; } = static _ =>
        Task.CompletedTask;
#else
    /// <summary>
    /// Called after the status code and headers have been set, but before the body has been written.
    /// This can be used to add or change the response headers.
    /// </summary>
    /// <remarks>
    /// <see cref="OnPrepareResponse" /> is called before <see cref="OnPrepareResponseAsync" />.
    /// </remarks>
    public Func<CompressedStaticFileResponseContext, Task> OnPrepareResponseAsync { get; set; } = static _ =>
        Task.CompletedTask;
#endif

#if NET8_0_OR_GREATER
    internal FrozenDictionary<StringSegment, int> ContentCodingOrder { get; private set; } = null!;
#else
    internal Dictionary<StringSegment, int> ContentCodingOrder { get; private set; } = null!;
#endif

    internal (string? ContentCoding, string? Extension)[] OrderLookup { get; private set; } = null!;

    internal StringValues AcceptEncoding { get; private set; }

    internal void Initialize()
    {
        AcceptEncoding = new StringValues(string.Join(',', ContentCoding.Select(static pair => pair.Key)));

        int i = 2;

        OrderLookup = new (string? ContentCoding, string? Extension)[ContentCoding.Count + i];
        var entries = new KeyValuePair<StringSegment, int>[ContentCoding.Count + i];
        entries[0] = new KeyValuePair<StringSegment, int>(ContentCodingNames.Identity, 0);
        entries[1] = new KeyValuePair<StringSegment, int>("*", 1);

        foreach ((string contentCoding, (_, string extension)) in
            ContentCoding.OrderBy(static pair => pair.Value.Order))
        {
            entries[i] = new KeyValuePair<StringSegment, int>(contentCoding, i);
            OrderLookup[i] = (contentCoding, extension);
            ++i;
        }

#if NET8_0_OR_GREATER
        ContentCodingOrder = entries.ToFrozenDictionary(StringSegmentComparer.OrdinalIgnoreCase);
#else
        ContentCodingOrder = new Dictionary<StringSegment, int>(entries, StringSegmentComparer.OrdinalIgnoreCase);
#endif
    }
}
