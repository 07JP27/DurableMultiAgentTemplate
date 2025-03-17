using System.ComponentModel;

namespace DurableMultiAgentTemplate.Agent.GetDestinationSuggestAgent;

public record GetDestinationSuggestRequest(
    [property: Description("行き先に求める希望の条件")]
    string SearchTerm);
