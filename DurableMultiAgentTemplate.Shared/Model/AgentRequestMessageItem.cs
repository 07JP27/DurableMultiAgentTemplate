using System.ComponentModel;

namespace DurableMultiAgentTemplate.Shared.Model;

public class AgentRequestMessageItem
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}