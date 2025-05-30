using System.ComponentModel;

namespace DurableMultiAgentTemplate.Agent.GetDestinationSuggestAgent;

/// <summary>
/// Record representing a request to suggest travel destinations.
/// Used as input for the GetDestinationSuggestActivity.
/// </summary>
public record GetDestinationSuggestRequest(
    [property: Description("行き先に求める希望の条件")]
    string SearchTerm);
