using System.ComponentModel;

namespace DurableMultiAgentTemplate.Shared.Model;

public class AdditionalMarkdownInfo : IAdditionalInfo
{
    [Description("Markdown形式の補足情報")]
    public string MarkdownText { get; set; } = "";
}
