namespace DurableMultiAgentTemplate.Shared.Model;

public class AgentRequestDto
{
    public List<AgentRequestMessageItem> Messages { get; set; } = default!;
    
    public bool RequireAdditionalInfo { get; set; } = false;
}