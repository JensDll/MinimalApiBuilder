using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.Net.Http.Headers;

// ReSharper disable CompareOfFloatsByEqualityOperator

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

        var acceptEncoding = Unsafe.As<List<StringWithQualityHeaderValue>>(requestHeaders.AcceptEncoding);

        foreach (StringWithQualityHeaderValue value in acceptEncoding)
        {
            if (!options.ContentCodingOrder.TryGetValue(value.Value, out int order))
            {
                continue;
            }

            visited[order] = true;

            double quality = value.Quality.GetValueOrDefault(1);

            if (quality < double.Epsilon)
            {
                // https://www.rfc-editor.org/rfc/rfc9110.html#section-12.5.3-10.2
                // If the representation has no content coding, then it is acceptable by default unless specifically
                // excluded by the Accept-Encoding header field stating either "identity;q=0" or "*;q=0" ...
                identityAllowed |= order is 0 or 1 ? IdentityAllowedFlags.NotAllowed : IdentityAllowedFlags.None;
                continue;
            }

            if (order == 0)
            {
                // ... without a more specific entry for "identity".
                identityAllowed |= IdentityAllowedFlags.Allowed;

                if (!identityExists)
                {
                    continue;
                }
            }

            if (quality > bestQuality)
            {
                bestOrder = order;
                bestQuality = quality;
            }
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

#pragma warning disable CS8762 // Only options.OrderLookup[0] and [1] contain null values

        // Best coding != "*"
        if (bestOrder != 1)
        {
            (contentCoding, extension) = options.OrderLookup[bestOrder];
            return true;
        }

        // Best coding == "*"
        Debug.Assert(visited[1]);

        // https://www.rfc-editor.org/rfc/rfc9110.html#section-12.5.3-6
        // The asterisk "*" symbol in an Accept-Encoding field matches any available content coding not explicitly listed in the field.
        // Note: It will not serve identity even if it isn't explicitly listed.
        for (int i = options.OrderLookup.Length - 1; i >= 2; --i)
        {
            if (visited[i])
            {
                continue;
            }

            (contentCoding, extension) = options.OrderLookup[i];
            return true;
        }

        (contentCoding, extension) = FindBestNonStarFallback(acceptEncoding, options, out bool success);
        return success;
    }

    private static (string?, string?) FindBestNonStarFallback(
        List<StringWithQualityHeaderValue> acceptEncoding,
        CompressedStaticFileOptions options,
        out bool success)
    {
        int bestOrder = -1;
        double bestQuality = -1;

        foreach (StringWithQualityHeaderValue value in acceptEncoding)
        {
            if (!options.ContentCodingOrder.TryGetValue(value.Value, out int order))
            {
                continue;
            }

            if (order == 1)
            {
                continue;
            }

            double quality = value.Quality.GetValueOrDefault(1);

            if (quality < double.Epsilon)
            {
                continue;
            }

            if (quality > bestQuality)
            {
                bestOrder = order;
                bestQuality = quality;
            }
            else if (quality == bestQuality && order > bestOrder)
            {
                bestOrder = order;
            }
        }

        success = bestOrder >= 2;
        return options.OrderLookup[bestOrder >= 2 ? bestOrder : 0];
    }
}
