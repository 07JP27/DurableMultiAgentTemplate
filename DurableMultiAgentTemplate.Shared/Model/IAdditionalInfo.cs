using System.Text.Json.Serialization;

namespace DurableMultiAgentTemplate.Shared.Model;

/// <summary>
/// Interface for additional information.
/// Classes implementing this interface are used as supplementary information added to agent responses.
/// </summary>
[JsonDerivedType(typeof(AdditionalMarkdownInfo), typeDiscriminator: "markdown")]
[JsonDerivedType(typeof(AdditionalLinkInfo), typeDiscriminator: "link")]
public interface IAdditionalInfo;
