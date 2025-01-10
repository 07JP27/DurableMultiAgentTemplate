using System.ComponentModel;

namespace DurableMultiAgentTemplate.Model;

public class GetDestinationSuggestRequest
{
    [Description("行き先に求める希望の条件")]
    public required string SearchTerm { get; set; } = string.Empty;
}
