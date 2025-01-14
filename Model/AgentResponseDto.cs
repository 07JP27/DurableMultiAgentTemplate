using System.Text.Json.Serialization;

namespace DurableMultiAgentTemplate.Model;

public class AgentResponseDto
{
    public string Content { get; set; } = string.Empty;

    public List<string> CaledAgentNames { get; set; } = new List<string>();

}