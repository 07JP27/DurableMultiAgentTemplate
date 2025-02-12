using DurableMultiAgentTemplate.Shared.Model;

namespace DurableMultiAgentTemplate;

public class AgentResponseWithAdditionalInfoFormat
{
    public string Content { get; set; } = "";
    public List<IAdditionalInfo> AdditionalInfo { get; set; } = new List<IAdditionalInfo>();
}