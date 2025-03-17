using System.ComponentModel;

namespace DurableMultiAgentTemplate.Shared.Model;

public record AdditionalLinkInfo(
    [property: Description("リンクのラベルとして表示されるテキスト")]
    string LinkText,
    [property: Description("リンク先のURL")]
    Uri Uri) : IAdditionalInfo;
