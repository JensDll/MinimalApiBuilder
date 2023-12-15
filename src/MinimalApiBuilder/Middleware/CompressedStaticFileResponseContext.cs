using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace MinimalApiBuilder.Middleware;

/// <summary>
/// Contains the current request context and the to-be-served filename without any content coding extension.
/// </summary>
public class CompressedStaticFileResponseContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompressedStaticFileResponseContext" /> class.
    /// </summary>
    /// <param name="context">The HTTP context associated with the request.</param>
    /// <param name="filename">The filename without any content coding extension.</param>
    public CompressedStaticFileResponseContext(HttpContext context, StringSegment filename)
    {
        Context = context;
        Filename = filename;
    }

    /// <summary>
    /// The HTTP context associated with the request.
    /// </summary>
    public HttpContext Context { get; }

    /// <summary>
    /// The filename without any content coding extension.
    /// </summary>
    public StringSegment Filename { get; }
}
