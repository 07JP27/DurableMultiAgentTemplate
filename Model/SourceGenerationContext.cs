using System.Text.Json.Serialization;

namespace DurableMultiAgentTemplate.Model;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(GetClimateRequest))]
[JsonSerializable(typeof(GetDestinationSuggestRequest))]
[JsonSerializable(typeof(GetHotelRequest))]
[JsonSerializable(typeof(GetSightseeingSpotRequest))]
[JsonSerializable(typeof(SubmitReservationRequest))]
[JsonSerializable(typeof(IAdditionalInfo))]
[JsonSerializable(typeof(AdditionalMarkdownInfo))]
[JsonSerializable(typeof(AdditionalLinkInfo))]
[JsonSerializable(typeof(AgentResponseWithAdditionalInfoFormat))]
internal partial class SourceGenerationContext : JsonSerializerContext;