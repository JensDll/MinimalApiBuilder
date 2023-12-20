using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace MinimalApiBuilder.Middleware;

/// <summary>
/// Contains the current HTTP context and selected information of the to-be-served file.
/// </summary>
public class CompressedStaticFileResponseContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompressedStaticFileResponseContext" /> class.
    /// </summary>
    /// <param name="context">The HTTP context associated with the request.</param>
    /// <param name="filename">The to-be-served file's name without any content coding extension.</param>
    /// <param name="contentCoding">
    /// The to-be-served file's content coding or <c>null</c> if no pre-compressed file is being served.
    /// </param>
    public CompressedStaticFileResponseContext(HttpContext context, StringSegment filename, string? contentCoding)
    {
        Context = context;
        Filename = filename;
        ContentCoding = contentCoding;
    }

    /// <summary>
    /// The HTTP context associated with the request.
    /// </summary>
    public HttpContext Context { get; }

    /// <summary>
    /// The to-be-served file's name without any content coding extension.
    /// </summary>
    public StringSegment Filename { get; }

    /// <summary>
    /// The to-be-served file's content coding or <c>null</c> if no pre-compressed file is being served.
    /// </summary>
    public string? ContentCoding { get; }
}
