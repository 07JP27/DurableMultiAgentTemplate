using System.Text.Json.Serialization;

namespace DurableMultiAgentTemplate.Shared.Model;

/// <summary>
/// 補足情報のインターフェース。
/// このインターフェースを実装したクラスは、エージェントからの応答に追加される補足情報として使用されます。
/// </summary>
[JsonDerivedType(typeof(AdditionalMarkdownInfo), typeDiscriminator: "markdown")]
[JsonDerivedType(typeof(AdditionalLinkInfo), typeDiscriminator: "link")]
public interface IAdditionalInfo;
