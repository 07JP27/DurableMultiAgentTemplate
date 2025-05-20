using System.ComponentModel;

namespace DurableMultiAgentTemplate.Shared.Model;

/// <summary>
/// Markdown形式の補足情報を表すレコード。
/// エージェントの回答に追加情報としてMarkdown形式のテキストを提供する場合に使用されます。
/// </summary>
public record AdditionalMarkdownInfo(
    [property: Description("Markdown形式の補足情報")]
    string MarkdownText) : IAdditionalInfo;
