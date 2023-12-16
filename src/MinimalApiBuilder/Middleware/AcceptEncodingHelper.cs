using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace MinimalApiBuilder.Middleware;

internal static class AcceptEncodingHelper
{
    // https://www.rfc-editor.org/rfc/rfc9110.html#section-12.5.3-9
    public static bool TryGetContentCoding(
        RequestHeaders requestHeaders,
        CompressedStaticFileOptions options,
        [NotNullWhen(true)] out string? contentCoding,
        [NotNullWhen(true)] out string? extension,
        out bool uncompressedFileAllowed)
    {
        uncompressedFileAllowed = true;
        contentCoding = null;
        extension = null;

        int bestOrder = -1;
        double bestQuality = -1;

        Span<bool> visited = stackalloc bool[options.OrderLookup.Length];

        foreach (StringWithQualityHeaderValue value in requestHeaders.AcceptEncoding)
        {
            double quality = value.Quality.GetValueOrDefault(1);

            if (quality < double.Epsilon)
            {
                if (StringSegment.Equals("*", value.Value, StringComparison.Ordinal) ||
                    StringSegment.Equals(ContentCodingNames.Identity, value.Value, StringComparison.OrdinalIgnoreCase))
                {
                    uncompressedFileAllowed = false;
                }

                continue;
            }

            if (!options.ContentCodingOrder.TryGetValue(value.Value, out int order))
            {
                continue;
            }

            visited[order] = true;

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

        // No codings matching options.ContentCodingOrder or all codings are q=0
        if (bestOrder == -1)
        {
            return false;
        }

#pragma warning disable CS8762 // Only options.OrderLookup[0] contains null values

        // Best coding != "*"
        if (bestOrder != 0)
        {
            (contentCoding, extension) = options.OrderLookup[bestOrder];
            return true;
        }

        // Best coding == "*"

        Debug.Assert(visited[0]);

        for (int i = options.OrderLookup.Length - 1; i >= 1; --i)
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
