using System.ComponentModel;

namespace DurableMultiAgentTemplate.Agent.GetClimateAgent;

public class GetClimateRequest
{
    [Description("場所の名前。例: ボストン, 東京、フランス")]
    public required string Location { get; set; } = string.Empty;
}
