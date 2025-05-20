using System.ComponentModel;

namespace DurableMultiAgentTemplate.Shared.Model;

/// <summary>
/// Record representing additional information in Markdown format.
/// Used when providing text in Markdown format as additional information to agent responses.
/// </summary>
public record AdditionalMarkdownInfo(
    [property: Description("Additional information in Markdown format")]
    string MarkdownText) : IAdditionalInfo;
