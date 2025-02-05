namespace DurableMultiAgentTemplate.Model;

public class AgentResponseWithAdditionalInfoDto:AgentResponseDto
{
    public List<IAdditionalInfo> AdditionalInfo { get; set; } = new List<IAdditionalInfo>();
}