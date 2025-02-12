using System.Text.Json.Serialization;

namespace DurableMultiAgentTemplate.Shared.Model;

[JsonDerivedType(typeof(AdditionalMarkdownInfo), typeDiscriminator: "markdown")]
[JsonDerivedType(typeof(AdditionalLinkInfo), typeDiscriminator: "link")]
public interface IAdditionalInfo
{
}
