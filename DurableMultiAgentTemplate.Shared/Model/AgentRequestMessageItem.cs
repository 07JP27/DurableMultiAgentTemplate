namespace DurableMultiAgentTemplate.Shared.Model;

/// <summary>
/// Represents a message item for an agent request.
/// </summary>
/// <param name="Role">The role of the agent.</param>
/// <param name="Content">The content of the message.</param>
public record AgentRequestMessageItem(
    string Role,
    string Content);
