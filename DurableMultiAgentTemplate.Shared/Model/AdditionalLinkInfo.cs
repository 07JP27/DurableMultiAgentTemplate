using System.ComponentModel;

namespace DurableMultiAgentTemplate.Shared.Model;

/// <summary>
/// Record representing additional information in link format.
/// Used when providing links as additional information to agent responses.
/// </summary>
public record AdditionalLinkInfo(
    [property: Description("Text displayed as the link label")]
    string LinkText,
    [property: Description("Destination URL of the link")]
    Uri Uri) : IAdditionalInfo;
