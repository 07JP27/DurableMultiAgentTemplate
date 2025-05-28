using System.ComponentModel;

namespace DurableMultiAgentTemplate.Agent.GetSightseeingSpotAgent;

/// <summary>
/// Record representing a request to retrieve sightseeing spot information.
/// Used as input for the GetSightseeingSpotActivity.
/// </summary>
public record GetSightseeingSpotRequest(
    [property: Description("場所の名前。例: ボストン, 東京、フランス")]
    string Location);
