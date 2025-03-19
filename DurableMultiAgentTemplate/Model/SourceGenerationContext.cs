using System.Text.Json.Serialization;
using DurableMultiAgentTemplate.Agent.Synthesizer;
using DurableMultiAgentTemplate.Agent.Workers.GetClimateAgent;
using DurableMultiAgentTemplate.Agent.Workers.GetDestinationSuggestAgent;
using DurableMultiAgentTemplate.Agent.Workers.GetHotelAgent;
using DurableMultiAgentTemplate.Agent.Workers.GetSightseeingSpotAgent;
using DurableMultiAgentTemplate.Agent.Workers.HotelReservationAgent;
using DurableMultiAgentTemplate.Shared.Model;

namespace DurableMultiAgentTemplate.Model;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(GetClimateRequest))]
[JsonSerializable(typeof(GetDestinationSuggestRequest))]
[JsonSerializable(typeof(GetHotelRequest))]
[JsonSerializable(typeof(GetSightseeingSpotRequest))]
[JsonSerializable(typeof(HotelReservationRequest))]
[JsonSerializable(typeof(IAdditionalInfo))]
[JsonSerializable(typeof(AdditionalMarkdownInfo))]
[JsonSerializable(typeof(AdditionalLinkInfo))]
[JsonSerializable(typeof(AgentResponseWithAdditionalInfoFormat))]
internal partial class SourceGenerationContext : JsonSerializerContext;
