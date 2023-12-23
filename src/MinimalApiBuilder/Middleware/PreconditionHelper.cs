using Microsoft.AspNetCore.Http.Headers;
using Microsoft.Net.Http.Headers;

namespace MinimalApiBuilder.Middleware;

internal static class PreconditionHelper
{
    public static (bool, bool) EvaluateIfRange(RequestHeaders requestHeaders,
        EntityTagHeaderValue etag, DateTimeOffset lastModified)
    {
        RangeConditionHeaderValue? ifRange = requestHeaders.IfRange;

        // https://www.rfc-editor.org/rfc/rfc9110.html#name-if-range
        return ifRange?.EntityTag is not null
            ? (true, ifRange.EntityTag.Compare(etag, true))
            : ifRange?.LastModified is not null
                ? (true, ifRange.LastModified == lastModified)
                : (false, false);
    }

    public static PreconditionState EvaluatePreconditions(RequestHeaders requestHeaders,
        EntityTagHeaderValue etag, DateTimeOffset lastModified)
    {
        return EvaluateIfMatch(requestHeaders, etag, lastModified);
    }

    private static PreconditionState EvaluateIfMatch(RequestHeaders requestHeaders,
        EntityTagHeaderValue etag, DateTimeOffset lastModified)
    {
        IList<EntityTagHeaderValue> ifMatch = requestHeaders.IfMatch;
        DateTimeOffset? ifUnmodifiedSince = requestHeaders.IfUnmodifiedSince;

        return ifMatch.Count > 0
            // https://www.rfc-editor.org/rfc/rfc9110.html#name-if-match
            ? ifMatch[0].Compare(EntityTagHeaderValue.Any, true) ||
              ifMatch.Any(value => value.Compare(etag, true))
                ? EvaluateIfNoneMatch(requestHeaders, etag, lastModified)
                : PreconditionState.PreconditionFailed
            // https://www.rfc-editor.org/rfc/rfc9110.html#name-if-unmodified-since
            : ifUnmodifiedSince.HasValue
                ? lastModified <= ifUnmodifiedSince.Value
                    ? EvaluateIfNoneMatch(requestHeaders, etag, lastModified)
                    : PreconditionState.PreconditionFailed
                : EvaluateIfNoneMatch(requestHeaders, etag, lastModified);
    }

    private static PreconditionState EvaluateIfNoneMatch(RequestHeaders requestHeaders,
        EntityTagHeaderValue etag, DateTimeOffset lastModified)
    {
        IList<EntityTagHeaderValue> ifNoneMatch = requestHeaders.IfNoneMatch;
        DateTimeOffset? ifModifiedSince = requestHeaders.IfModifiedSince;

        return ifNoneMatch.Count > 0
            // https://www.rfc-editor.org/rfc/rfc9110.html#name-if-none-match
            ? ifNoneMatch[0].Compare(EntityTagHeaderValue.Any, false) ||
              ifNoneMatch.Any(value => value.Compare(etag, false))
                ? PreconditionState.NotModified
                : PreconditionState.ShouldProcess
            // https://www.rfc-editor.org/rfc/rfc9110.html#name-if-modified-since
            : ifModifiedSince.HasValue
                ? lastModified <= ifModifiedSince.Value
                    ? PreconditionState.NotModified
                    : PreconditionState.ShouldProcess
                : PreconditionState.ShouldProcess;
    }
}
