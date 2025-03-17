using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace DurableMultiAgentTemplate.Shared.Model;

/// <summary>
/// Represents the response from an agent with additional information.
/// </summary>
/// <param name="Content">The content of the response.</param>
/// <param name="CalledAgentNames">The list of names of the agents that were called.</param>
/// <param name="AdditionalInfo">The additional information related to the response.</param>
[method: JsonConstructor]
public record AgentResponseWithAdditionalInfoDto(string Content,
    List<string> CalledAgentNames,
    List<IAdditionalInfo> AdditionalInfo) : AgentResponseDto(Content, CalledAgentNames)
{
    public AgentResponseWithAdditionalInfoDto(string content) : this(content, [], [])
    {
    }
}
