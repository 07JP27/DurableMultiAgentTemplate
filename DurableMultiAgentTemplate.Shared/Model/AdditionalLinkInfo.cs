using System.ComponentModel;

namespace DurableMultiAgentTemplate.Shared.Model;

public class AdditionalLinkInfo : IAdditionalInfo
{
    [Description("リンクのラベルとして表示されるテキスト")]
    public string LinkText { get; set; } = "";

    [Description("リンク先のURL")]
    public required Uri Uri { get; set; }
}
