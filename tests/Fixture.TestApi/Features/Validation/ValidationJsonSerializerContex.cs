using System.Text.Json.Serialization;

namespace Fixture.TestApi.Features.Validation;

[JsonSerializable(typeof(AsyncValidationRequest))]
[JsonSerializable(typeof(SyncValidationRequest))]
internal partial class ValidationJsonSerializerContext : JsonSerializerContext
{ }
