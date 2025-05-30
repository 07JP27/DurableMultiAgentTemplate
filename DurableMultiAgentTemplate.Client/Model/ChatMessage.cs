using DurableMultiAgentTemplate.Shared.Model;

namespace DurableMultiAgentTemplate.Client.Model;

/// <summary>
/// Abstract record representing a chat message in the conversation.
/// Base class for different types of messages in the chat interface.
/// </summary>
/// <param name="Role">The role of the message sender.</param>
/// <param name="IsRequestTarget">Indicates whether the message is a target for a request.</param>
public abstract record ChatMessage(Role Role, bool IsRequestTarget);

/// <summary>
/// Record representing a message from the user.
/// </summary>
/// <param name="Message">The content of the user's message.</param>
public record UserChatMessage(string Message) : ChatMessage(Role.User, true);

/// <summary>
/// Record representing a message from the agent.
/// Contains the response data including any additional information.
/// </summary>
/// <param name="Message">The agent's response data.</param>
public record AgentChatMessage(AgentResponseWithAdditionalInfoDto Message) : ChatMessage(Role.Assistant, true);

/// <summary>
/// Record representing an informational message in the chat.
/// Used for system messages and status updates.
/// </summary>
/// <param name="Info">The informational text.</param>
/// <param name="IsShowProgress">Indicates whether to display progress information.</param>
public record InfoChatMessage(string Info, bool IsShowProgress) : ChatMessage(Role.Info, false);

/// <summary>
/// Enum defining the possible roles in a conversation.
/// </summary>
public enum Role
{
    /// <summary>
    /// Represents a human user.
    /// </summary>
    User,
    
    /// <summary>
    /// Represents an AI assistant.
    /// </summary>
    Assistant,
    
    /// <summary>
    /// Represents an informational system message.
    /// </summary>
    Info
}

/// <summary>
/// Extension methods for the Role enum.
/// </summary>
public static class RoleExtensions
{
    /// <summary>
    /// Converts a Role enum value to its string representation.
    /// </summary>
    /// <param name="role">The role to convert.</param>
    /// <returns>String representation of the role.</returns>
    /// <exception cref="InvalidOperationException">Thrown when an unsupported role is provided.</exception>
    public static string ToRoleName(this Role role) => role switch
    {
        Role.User => "user",
        Role.Assistant => "assistant",
        _ => throw new InvalidOperationException()
    };
}
