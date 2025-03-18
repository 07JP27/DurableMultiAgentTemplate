using DurableMultiAgentTemplate.Shared.Model;

namespace DurableMultiAgentTemplate.Agent.Workers;

/// <summary>
/// Represents the result of a worker agent's operation.
/// </summary>
/// <param name="Content">The content produced by the worker agent.</param>
/// <param name="NextAgentCall">The next agent call to be made, if any.</param>
public record WorkerAgentResult(string Content,
    AgentCall? NextAgentCall)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WorkerAgentResult"/> class.
    /// </summary>
    /// <param name="content">The content produced by the worker agent.</param>
    public static implicit operator WorkerAgentResult(string content) => new (content, null);
}
