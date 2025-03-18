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

[method: JsonConstructor]
public record AgentMessageItem(AgentMessageType AgentMessageType,
    string Content,
    AgentCall? NextAgentCall) : MessageItem(AgentRole.Agent, Content)
{
    public AgentMessageItem(string Content) : this(AgentMessageType.Info, Content, null)
    {
    }

    public AgentMessageItem(string Content, AgentCall NextAgentCall) : this(AgentMessageType.Ask, Content, NextAgentCall)
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

/// <summary>
/// Represents a message type for an agent.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AgentMessageType
{
    /// <summary>
    /// Represents a question to the user.
    /// </summary>
    Ask,
    /// <summary>
    /// Represents general information.
    /// </summary>
    Info
}
