using System.ComponentModel;

namespace DurableMultiAgentTemplate.Shared.Model;

public class AgentResponseWithAdditionalInfoFormat
{
    public string Content { get; set; } = "";
    public List<IAdditionalInfo> AdditionalInfo { get; set; } = new List<IAdditionalInfo>();
}