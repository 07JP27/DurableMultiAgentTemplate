using System.Text.Json.Serialization;

namespace DurableMultiAgentTemplate.Shared.Model;

/// <summary>
/// Represents the response from an agent.
/// </summary>
/// <param name="Item">The content of the response.</param>
/// <param name="CalledAgentNames">The list of names of the agents that were called.</param>
[method: JsonConstructor]
public record AgentResponseDto(
    AgentMessageItem Item,
    List<string> CalledAgentNames)
{
    public AgentResponseDto(AgentMessageItem content) : this(content, [])
    {
    }
}
