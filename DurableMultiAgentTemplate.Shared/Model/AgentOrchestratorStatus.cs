using System.Text.Json.Serialization;

namespace DurableMultiAgentTemplate.Shared.Model;

/// <summary>
/// Represents the status of the agent orchestrator.
/// </summary>
/// <param name="Step">The current step of the agent orchestrator.</param>
/// <param name="CustomStatus">The custom status of the agent orchestrator.</param>
[method: JsonConstructor]
public record AgentOrchestratorStatus(
    AgentOrchestratorStep Step,
    IEnumerable<AgentCall> AgentCalls)
{
    /// <summary>
    /// The orchestrator has not started.
    /// </summary>
    public static AgentOrchestratorStatus NotStarted { get; } = new(AgentOrchestratorStep.NotStarted);

    /// <summary>
    /// Initializes a new instance of the <see cref="AgentOrchestratorStatus"/> class.
    /// </summary>
    /// <param name="step">The current step of the agent orchestrator.</param>
    public AgentOrchestratorStatus(AgentOrchestratorStep step)
        : this(step, [])
    {
    }
}

/// <summary>
/// Defines the steps of the agent orchestrator.
/// </summary>
public enum AgentOrchestratorStep
{
    /// <summary>
    /// The orchestrator has not started.
    /// </summary>
    NotStarted,

    /// <summary>
    /// The step where the agent decides the activity.
    /// </summary>
    AgentDeciderActivity,

    /// <summary>
    /// The step where the agent calls the activity.
    /// </summary>
    WorkerAgentActivity,

    /// <summary>
    /// The step where the synthesizer activity occurs.
    /// </summary>
    SynthesizerActivity
};
