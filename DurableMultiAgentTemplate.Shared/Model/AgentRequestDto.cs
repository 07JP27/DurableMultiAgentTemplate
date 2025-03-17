namespace DurableMultiAgentTemplate.Shared.Model;

/// <summary>
/// Represents a data transfer object for agent requests.
/// </summary>
/// <param name="Messages">A list of messages associated with the agent request.</param>
/// <param name="RequireAdditionalInfo">Indicates whether additional information is required for the agent request.</param>
public record AgentRequestDto(
    List<AgentRequestMessageItem> Messages,
    bool RequireAdditionalInfo = false);
