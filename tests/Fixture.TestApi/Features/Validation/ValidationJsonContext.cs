#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Fixture.TestApi.Features.Validation;

[JsonSerializable(typeof(AsyncValidationRequest))]
[JsonSerializable(typeof(SyncValidationRequest))]
[JsonSerializable(typeof(Dictionary<string, string[]>))]
internal partial class ValidationJsonContext : JsonSerializerContext;
#endif
