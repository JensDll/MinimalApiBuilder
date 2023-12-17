using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.Net.Http.Headers;

namespace MinimalApiBuilder.Middleware;

internal static class AcceptEncodingHelper
{
    // https://www.rfc-editor.org/rfc/rfc9110.html#section-12.5.3-9
    public static bool TryGetContentCoding(
        RequestHeaders requestHeaders,
        CompressedStaticFileOptions options,
        bool identityExists,
        [NotNullWhen(true)] out string? contentCoding,
        [NotNullWhen(true)] out string? extension,
        out IdentityAllowedFlags identityAllowed)
    {
        identityAllowed = identityExists ? IdentityAllowedFlags.None : IdentityAllowedFlags.Allowed;
        contentCoding = null;
        extension = null;

        int bestOrder = -1;
        double bestQuality = -1;

        Span<bool> visited = stackalloc bool[options.OrderLookup.Length];

        foreach (StringWithQualityHeaderValue value in requestHeaders.AcceptEncoding)
        {
            if (!options.ContentCodingOrder.TryGetValue(value.Value, out int order))
            {
                continue;
            }

            visited[order] = true;

            double quality = value.Quality.GetValueOrDefault(1);

            if (quality < double.Epsilon)
            {
                // If the representation has no content coding, then it is acceptable by default unless specifically
                // excluded by the Accept-Encoding header field stating either "identity;q=0" or "*;q=0" ...
                if (order is 0 or 1)
                {
                    identityAllowed |= IdentityAllowedFlags.NotAllowed;
                }

                continue;
            }

            // ... without a more specific entry for "identity"
            identityAllowed |= order is 0 or 1 ? IdentityAllowedFlags.Allowed : IdentityAllowedFlags.None;

            if (!identityExists && order == 0)
            {
                continue;
            }

            if (quality > bestQuality)
            {
                bestOrder = order;
                bestQuality = quality;
            }
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            // Save to compare as both doubles are never used in calculations
            else if (quality == bestQuality && order > bestOrder)
            {
                bestOrder = order;
            }
        }

        // No codings matching options.ContentCodingOrder, or is identity, or all codings are q=0
        if (bestOrder <= 0)
        {
            return false;
        }

#pragma warning disable CS8762 // Only options.OrderLookup[0] and [1] contains null values

        // Best coding != "*"
        if (bestOrder != 1)
        {
            (contentCoding, extension) = options.OrderLookup[bestOrder];
            return true;
        }

        // Best coding == "*"
        Debug.Assert(visited[1]);

        for (int i = options.OrderLookup.Length - 1; i >= 2; --i)
        {
            if (visited[i])
            {
                continue;
            }

            (contentCoding, extension) = options.OrderLookup[i];
            return true;
        }

        (contentCoding, extension) = options.OrderLookup[^1];
        return options.OrderLookup.Length != 1;
    }
}
