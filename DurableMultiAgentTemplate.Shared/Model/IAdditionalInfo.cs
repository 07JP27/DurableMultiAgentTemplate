using System.Text.Json.Serialization;

namespace DurableMultiAgentTemplate.Shared.Model;

/// <summary>
/// Interface for additional information.
/// Classes that implement this interface are used as supplementary information to be added to responses from agents.
/// </summary>
[JsonDerivedType(typeof(AdditionalMarkdownInfo), typeDiscriminator: "markdown")]
[JsonDerivedType(typeof(AdditionalLinkInfo), typeDiscriminator: "link")]
public interface IAdditionalInfo;
