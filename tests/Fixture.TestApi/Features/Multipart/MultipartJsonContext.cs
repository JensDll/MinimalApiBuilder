#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Fixture.TestApi.Features.Multipart;

[JsonSerializable(typeof(IEnumerable<BufferedFilesResponse>))]
internal partial class MultipartJsonContext : JsonSerializerContext;
#endif
