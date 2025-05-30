using System.Text.Json.Serialization;

namespace DurableMultiAgentTemplate.Shared.Model;

/// <summary>
/// Represents the response from an agent.
/// </summary>
/// <param name="Content">The content of the response.</param>
/// <param name="CalledAgentNames">The list of names of the agents that were called.</param>
[method: JsonConstructor]
public record AgentResponseDto(
    string Content,
    List<string> CalledAgentNames)
{
    /// <summary>
    /// Initializes a new instance of the AgentResponseDto class with empty called agent names.
    /// </summary>
    /// <param name="content">The content of the response.</param>
    public AgentResponseDto(string content) : this(content, [])
    {
    }
}
