using System.ComponentModel;

namespace DurableMultiAgentTemplate.Shared.Model;

/// <summary>
/// Record representing additional information in Markdown format.
/// Used when providing Markdown text as additional information in agent responses.
/// </summary>
public record AdditionalMarkdownInfo(
    [property: Description("Markdown形式の補足情報")]
    string MarkdownText) : IAdditionalInfo;
