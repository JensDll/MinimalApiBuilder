using System.Text.Json.Serialization;

namespace Fixture.TestApi.Features.Multipart;

[JsonSerializable(typeof(BufferedFilesResponse))]
internal partial class MultipartJsonSerializerContext : JsonSerializerContext
{ }
