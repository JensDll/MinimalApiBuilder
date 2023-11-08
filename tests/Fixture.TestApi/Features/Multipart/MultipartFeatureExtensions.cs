using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MinimalApiBuilder;

namespace Fixture.TestApi.Features.Multipart;

internal static class MultipartFeatureExtensions
{
    internal static void MapMultipartFeature(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder group = endpoints.MapGroup("/multipart").WithTags("Multipart");
        group.MapPost<ZipStreamEndpoint>("/zipstream");
        group.MapPost<BufferedFilesEndpoint>("/bufferedfiles");
    }
}
