using System.ComponentModel;

namespace DurableMultiAgentTemplate.Agent.GetClimateAgent;

public record GetClimateRequest(
    [property: Description("場所の名前。例: ボストン, 東京、フランス")]
    string Location);
