using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using MinimalApiBuilder;

namespace Fixture.TestApi.Features.Multipart;

internal partial class BufferedFilesEndpoint : MinimalApiBuilderEndpoint
{
    public static Results<Ok<IEnumerable<BufferedFilesResponse>>, BadRequest> Handle(BufferedFilesRequest request)
    {
        return TypedResults.Ok(request.Files.Select(static file =>
        {
            Stream stream = file.OpenReadStream();
            byte[] data = new byte[file.Length];
            stream.ReadExactly(data, 0, data.Length);
            return new BufferedFilesResponse
            {
                Name = file.FileName,
                Data = data
            };
        }));
    }
}

internal class BufferedFilesRequest
{
    public required IReadOnlyList<IFormFile> Files { get; init; }

    public static async ValueTask<BufferedFilesRequest?> BindAsync(HttpContext context)
    {
        CancellationToken cancellationToken = context.RequestAborted;
        ZipStreamEndpoint endpoint = context.RequestServices.GetRequiredService<ZipStreamEndpoint>();
        MultipartReader multipartReader = new(context, endpoint);

        if (endpoint.HasValidationError)
        {
            return null;
        }

        IReadOnlyList<IFormFile>? files = await multipartReader.ReadFilesBufferedAsync(cancellationToken);

        if (files is null)
        {
            return null;
        }

        return new BufferedFilesRequest
        {
            Files = files
        };
    }
}

internal class BufferedFilesResponse
{
    public required string Name { get; init; }

    public required byte[] Data { get; init; }
}

[JsonSerializable(typeof(BufferedFilesResponse))]
internal partial class BufferedFilesResponseJsonSerializerContext : JsonSerializerContext
{ }
