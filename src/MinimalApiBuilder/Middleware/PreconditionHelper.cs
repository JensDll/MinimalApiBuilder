using System.Runtime.CompilerServices;
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
        var ifMatch = Unsafe.As<List<EntityTagHeaderValue>>(requestHeaders.IfMatch);
        DateTimeOffset? ifUnmodifiedSince = requestHeaders.IfUnmodifiedSince;

        // https://www.rfc-editor.org/rfc/rfc9110.html#name-if-match
        // https://www.rfc-editor.org/rfc/rfc9110.html#name-if-unmodified-since

        return ifMatch.Count > 0
            ? ifMatch[0].Compare(EntityTagHeaderValue.Any, true) ||
              ifMatch.Contains(etag, true)
                ? EvaluateIfNoneMatch(requestHeaders, etag, lastModified)
                : PreconditionState.PreconditionFailed
            : ifUnmodifiedSince.HasValue
                ? lastModified <= ifUnmodifiedSince.Value
                    ? EvaluateIfNoneMatch(requestHeaders, etag, lastModified)
                    : PreconditionState.PreconditionFailed
                : EvaluateIfNoneMatch(requestHeaders, etag, lastModified);
    }

    private static PreconditionState EvaluateIfNoneMatch(RequestHeaders requestHeaders,
        EntityTagHeaderValue etag, DateTimeOffset lastModified)
    {
        var ifNoneMatch = Unsafe.As<List<EntityTagHeaderValue>>(requestHeaders.IfNoneMatch);
        DateTimeOffset? ifModifiedSince = requestHeaders.IfModifiedSince;

        // https://www.rfc-editor.org/rfc/rfc9110.html#name-if-none-match
        // https://www.rfc-editor.org/rfc/rfc9110.html#name-if-modified-since

        return ifNoneMatch.Count > 0
            ? ifNoneMatch[0].Compare(EntityTagHeaderValue.Any, false) ||
              ifNoneMatch.Contains(etag, false)
                ? PreconditionState.NotModified
                : PreconditionState.ShouldProcess
            : ifModifiedSince.HasValue
                ? lastModified <= ifModifiedSince.Value
                    ? PreconditionState.NotModified
                    : PreconditionState.ShouldProcess
                : PreconditionState.ShouldProcess;
    }

    private static bool Contains(this List<EntityTagHeaderValue> etags,
        EntityTagHeaderValue etag, bool useStrongComparison)
    {
        foreach (EntityTagHeaderValue value in etags)
        {
            if (value.Compare(etag, useStrongComparison))
            {
                return true;
            }
        }

        return false;
    }
}
