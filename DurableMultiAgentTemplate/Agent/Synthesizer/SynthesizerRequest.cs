using DurableMultiAgentTemplate.Agent.Workers;
using DurableMultiAgentTemplate.Shared.Model;

namespace DurableMultiAgentTemplate.Agent.Synthesizer;

/// <summary>
/// Represents a request to the synthesizer.
/// </summary>
/// <param name="AgentCallResult">The results of the agent calls.</param>
/// <param name="AgentRequest">The agent request details.</param>
/// <param name="CalledAgentNames">The names of the called agents.</param>
public record SynthesizerRequest(
    List<WorkerAgentResult> AgentCallResult,
    AgentRequestDto AgentRequest,
    List<string> CalledAgentNames);
