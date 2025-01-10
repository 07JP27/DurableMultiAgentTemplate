using System.Text.Json.Serialization;

namespace DurableMultiAgentTemplate.Model;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(GetClimateRequest))]
[JsonSerializable(typeof(GetDestinationSuggestRequest))]
[JsonSerializable(typeof(GetHotelRequest))]
[JsonSerializable(typeof(GetSightseeingSpotRequest))]
[JsonSerializable(typeof(SubmitReservationRequest))]
internal partial class SourceGenerationContext : JsonSerializerContext;