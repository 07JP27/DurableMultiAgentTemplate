using System.ComponentModel;
using System.Text.Json.Serialization;

namespace DurableMultiAgentTemplate.Model;

public class AgentResponseWithAdditionalInfoDto:AgentResponseDto
{
    public List<AdditionalInfo> AdditionalInfo { get; set; } = new List<AdditionalInfo>();
}

public class AgentResponseWithAdditionalInfoFormat
{
    public string Content { get; set; } = "";
    public List<AdditionalInfo> AdditionalInfo { get; set; } = new List<AdditionalInfo>();
}

[JsonDerivedType(typeof(AdditionalMarkdownInfo), typeDiscriminator: "markwodn")]
[JsonDerivedType(typeof(AdditionalLinkInfo), typeDiscriminator: "link")]
public class AdditionalInfo
{
    public string Title { get; set; } = "";
}

class AdditionalMarkdownInfo : AdditionalInfo
{
    [Description("Markdown形式のテキスト")]
    public string MarkdownText { get; set; } = "";
}

class AdditionalLinkInfo : AdditionalInfo
{
    [Description("リンクのラベルとして表示されるテキスト")]
    public string LinkText { get; set; } = "";

    [Description("リンク先のURL")]
    public required Uri Uri { get; set; }
}