using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace Fixture.TestApi.Common;

internal static class Headers
{
    private static readonly ContentDispositionHeaderValue s_resultZipContentDisposition = new("attachment");

    static Headers()
    {
        s_resultZipContentDisposition.SetHttpFileName("result.zip");
    }

    public static readonly StringValues NoSniff = new("nosniff");

    public static readonly StringValues CacheControl = new("public,max-age=31536000,immutable");

    public static readonly StringValues CacheControlHtml = new("private,no-store,no-cache,max-age=0,must-revalidate");

    public static readonly StringValues XXSSProtection = new("0");

    public static readonly StringValues ResultZip = new(s_resultZipContentDisposition.ToString());
}
