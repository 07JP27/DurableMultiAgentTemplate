using System.ComponentModel;

namespace DurableMultiAgentTemplate.Agent.GetDestinationSuggestAgent;

public class GetDestinationSuggestRequest
{
    [Description("行き先に求める希望の条件")]
    public required string SearchTerm { get; set; } = string.Empty;
}
