using System.ComponentModel;

namespace DurableMultiAgentTemplate.Agent.GetClimateAgent;

/// <summary>
/// Record representing a request to retrieve climate information.
/// Used as input for the GetClimateActivity.
/// </summary>
public record GetClimateRequest(
    [property: Description("場所の名前。例: ボストン, 東京、フランス")]
    string Location);
