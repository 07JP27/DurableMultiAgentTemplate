using System.ComponentModel;
using System.Text.Json.Serialization;

namespace DurableMultiAgentTemplate.Model;

public class AgentResponseWithAdditionalInfoFormat
{
    public string Content { get; set; } = "";
    public List<IAdditionalInfo> AdditionalInfo { get; set; } = new List<IAdditionalInfo>();
}

[JsonDerivedType(typeof(AdditionalMarkdownInfo), typeDiscriminator: "markdown")]
[JsonDerivedType(typeof(AdditionalLinkInfo), typeDiscriminator: "link")]
public interface IAdditionalInfo
{
}

class AdditionalMarkdownInfo : IAdditionalInfo
{
    [Description("Markdown形式の補足情報")]
    public string MarkdownText { get; set; } = "";
}

class AdditionalLinkInfo : IAdditionalInfo
{
    [Description("リンクのラベルとして表示されるテキスト")]
    public string LinkText { get; set; } = "";

    [Description("リンク先のURL")]
    public required Uri Uri { get; set; }
}