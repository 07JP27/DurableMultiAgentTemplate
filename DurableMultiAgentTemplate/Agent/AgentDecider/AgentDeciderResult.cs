using DurableMultiAgentTemplate.Shared.Model;

namespace DurableMultiAgentTemplate.Agent.AgentDecider;

public class AgentDeciderResult
{
    public bool IsAgentCall { get; set; }
    public string Content { get; set; } = string.Empty;
    public IList<AgentCall> AgentCalls { get; set; } = default!;

}
