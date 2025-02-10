using System.ComponentModel;

namespace DurableMultiAgentTemplate.Shared.Model;

public class AgentResponseDto
{
    public string Content { get; set; } = string.Empty;

    public List<string> CalledAgentNames { get; set; } = new List<string>();

}