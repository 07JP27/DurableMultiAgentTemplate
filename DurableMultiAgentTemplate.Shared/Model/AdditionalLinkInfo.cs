using System.ComponentModel;

namespace DurableMultiAgentTemplate.Shared.Model;

/// <summary>
/// Record representing additional information in link format.
/// Used when providing links as additional information in agent responses.
/// </summary>
public record AdditionalLinkInfo(
    [property: Description("リンクのラベルとして表示されるテキスト")]
    string LinkText,
    [property: Description("リンク先のURL")]
    Uri Uri) : IAdditionalInfo;
