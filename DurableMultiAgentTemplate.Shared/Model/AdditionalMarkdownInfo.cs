using System.ComponentModel;

namespace DurableMultiAgentTemplate.Shared.Model;

public record AdditionalMarkdownInfo(
    [property: Description("Markdown形式の補足情報")]
    string MarkdownText) : IAdditionalInfo;
