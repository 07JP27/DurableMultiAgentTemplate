using System.Text.Json.Serialization;

namespace DurableMultiAgentTemplate.Model;

public class AgentRequestDto
{
    public List<AgentRequestMessageItem> Messages { get; set; } = default!;
    
    public bool RequireSupplementInfo { get; set; } = false;
}

public class AgentRequestMessageItem
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}