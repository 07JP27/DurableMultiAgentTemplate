using System.Text.Json.Serialization;

namespace DurableMultiAgentTemplate.Shared.Model;

/// <summary>
/// Represents a message item for an agent request.
/// </summary>
/// <param name="Role">The role of the agent.</param>
/// <param name="Content">The content of the message.</param>
[JsonDerivedType(typeof(UserMessageItem), typeDiscriminator: "user")]
[JsonDerivedType(typeof(AgentMessageItem), typeDiscriminator: "agent")]
public abstract record MessageItem(
    AgentRole Role,
    string Content);

public record UserMessageItem(string Content) : MessageItem(AgentRole.User, Content);

/// <summary>
/// Represents a message item for an agent response.
/// </summary>
/// <param name="Content">The content of the message.</param>
/// <param name="NextAgentCall">Represents the next agent to be called.</param>
[method: JsonConstructor]
public record AgentMessageItem(string Content,
    AgentCall? NextAgentCall) : MessageItem(AgentRole.Agent, Content)
{
    public AgentMessageItem(string content) : this(content, null)
    {
    }
}

/// <summary>
/// Represents the role of an agent.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AgentRole
{
    /// <summary>
    /// Represents a user role.
    /// </summary>
    User,
    /// <summary>
    /// Represents a agent role.
    /// </summary>
    Agent
}
