using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;

namespace MinimalApiBuilder.Middleware;

internal static class EtagHelper
{
    private static readonly char[] s_etagChars =
    {
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
        'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
        'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '=', '-'
    };

    internal static (EntityTagHeaderValue, DateTimeOffset) GetEtagAndLastModified(IFileInfo fileInfo)
    {
        DateTimeOffset last = fileInfo.LastModified;
        // Truncate to second precision
        DateTimeOffset lastModified = new DateTimeOffset(last.Year, last.Month, last.Day,
            last.Hour, last.Minute, last.Second, last.Offset).ToUniversalTime();

        string etagValue = string.Create(13, lastModified.ToFileTime() ^ fileInfo.Length, static (span, hash) =>
        {
            span[0] = '"';
            span[1] = s_etagChars[hash & 63];
            span[2] = s_etagChars[(hash >> 6) & 63];
            span[3] = s_etagChars[(hash >> 12) & 63];
            span[4] = s_etagChars[(hash >> 18) & 63];
            span[5] = s_etagChars[(hash >> 24) & 63];
            span[6] = s_etagChars[(hash >> 30) & 63];
            span[7] = s_etagChars[(hash >> 36) & 63];
            span[8] = s_etagChars[(hash >> 42) & 63];
            span[9] = s_etagChars[(hash >> 48) & 63];
            span[10] = s_etagChars[(hash >> 54) & 63];
            span[11] = s_etagChars[(hash >> 60) & 63];
            span[12] = '"';
        });

        return (new EntityTagHeaderValue(etagValue), lastModified);
    }
}
