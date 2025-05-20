using System.ComponentModel;

namespace DurableMultiAgentTemplate.Shared.Model;

/// <summary>
/// リンク形式の補足情報を表すレコード。
/// エージェントの回答に追加情報としてリンクを提供する場合に使用されます。
/// </summary>
public record AdditionalLinkInfo(
    [property: Description("リンクのラベルとして表示されるテキスト")]
    string LinkText,
    [property: Description("リンク先のURL")]
    Uri Uri) : IAdditionalInfo;
