using System.Text.Json.Serialization;
using DurableMultiAgentTemplate.Agent.GetClimateAgent;
using DurableMultiAgentTemplate.Agent.GetDestinationSuggestAgent;
using DurableMultiAgentTemplate.Agent.GetHotelAgent;
using DurableMultiAgentTemplate.Agent.GetSightseeingSpotAgent;
using DurableMultiAgentTemplate.Agent.SubmitReservationAgent;
using DurableMultiAgentTemplate.Agent.Synthesizer;
using DurableMultiAgentTemplate.Shared.Model;

namespace DurableMultiAgentTemplate.Model;

/// <summary>
/// Source generation context for JSON serialization.
/// Configures the JSON serialization options and declares serializable types
/// for performance optimization through source generation.
/// </summary>
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
