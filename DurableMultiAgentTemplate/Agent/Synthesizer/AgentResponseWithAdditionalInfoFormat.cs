using DurableMultiAgentTemplate.Shared.Model;

namespace DurableMultiAgentTemplate.Agent.Synthesizer;

/// <summary>
/// Represents a response from an agent with additional information.
/// </summary>
/// <param name="Content">The main content of the response.</param>
/// <param name="AdditionalInfo">A list of additional information related to the response.</param>
public record AgentResponseWithAdditionalInfoFormat(
    string Content,
    List<IAdditionalInfo> AdditionalInfo);
