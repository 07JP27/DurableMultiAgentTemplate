using DurableMultiAgentTemplate.Shared.Model;

namespace DurableMultiAgentTemplate.Agent.AgentDecider;

/// <summary>
/// Represents the result of an agent decider operation.
/// </summary>
/// <param name="IsAgentCall">Indicates whether the result involves an agent call.</param>
/// <param name="Content">The content of the result.</param>
/// <param name="AgentCalls">A list of agent calls associated with the result.</param>
public record AgentDeciderResult(
    bool IsAgentCall,
    string Content,
    IList<AgentCall> AgentCalls);
