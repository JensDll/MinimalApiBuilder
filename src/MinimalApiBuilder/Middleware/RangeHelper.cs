using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace MinimalApiBuilder.Middleware;

internal static class RangeHelper
{
    public static bool HasRangeHeaderField(HttpContext context)
    {
        return !StringValues.IsNullOrEmpty(context.Request.Headers.Range);
    }

    public static bool TryParseRange(HttpContext context, RequestHeaders requestHeaders,
        long length, out (long, long)? range)
    {
        range = null;

        StringValues rawRange = context.Request.Headers.Range;

        if (StringValues.IsNullOrEmpty(rawRange))
        {
            return false;
        }

        // Disallow multiple ranges from multiple field lines
        if (rawRange.Count > 1)
        {
            return false;
        }

        Debug.Assert(rawRange.Count == 1);

        // Disallow multiple ranges from a single field line
        if (rawRange[0]!.Contains(',', StringComparison.Ordinal))
        {
            return false;
        }

        RangeHeaderValue? rangeHeader = requestHeaders.Range;

        // Failed to parse, ignore
        if (rangeHeader is null)
        {
            return false;
        }

        RangeItemHeaderValue singleRangeItem = rangeHeader.Ranges.Single();
        long? from = singleRangeItem.From;
        long? to = singleRangeItem.To;

        // Case 1: The n to m-byte or every remaining byte starting from n (int-range)
        if (from.HasValue) // Range: bytes=n-[m]
        {
            if (from.Value >= length)
            {
                return true; // range not satisfiable (416)
            }

            range = (from.Value, to.HasValue ? Math.Min(to.Value, length - 1) : length - 1);
        }
        // Case 2: The final n-byte (suffix-range)
        else if (to.HasValue) // Range: bytes=-n
        {
            if (to.Value == 0)
            {
                return true; // range not satisfiable (416)
            }

            range = (length - Math.Min(to.Value, length), length - 1);
        }

        return true;
    }
}
