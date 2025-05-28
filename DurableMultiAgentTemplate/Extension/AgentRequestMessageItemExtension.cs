using OpenAI.Chat;
using DurableMultiAgentTemplate.Shared.Model;

namespace DurableMultiAgentTemplate.Extension;

/// <summary>
/// Extension methods for AgentRequestMessageItem.
/// Provides conversion functionality between the application's message format and OpenAI's chat message format.
/// </summary>
public static class AgentRequestMessageItemExtension
{
    /// <summary>
    /// Converts a collection of AgentRequestMessageItem objects to ChatMessage objects.
    /// </summary>
    /// <param name="messages">The collection of message items to convert.</param>
    /// <returns>A collection of ChatMessage objects.</returns>
    /// <exception cref="InvalidOperationException">Thrown when an unsupported role is encountered.</exception>
    public static IEnumerable<ChatMessage> ConvertToChatMessageArray(this IEnumerable<AgentRequestMessageItem> messages)
    {
        return messages.Select<AgentRequestMessageItem, ChatMessage>(m =>
        {
            return m.Role switch
            {
                "user" => new UserChatMessage(m.Content),
                "assistant" => new AssistantChatMessage(m.Content),
                _ => throw new InvalidOperationException($"You can not set role: {m.Role}")
            };
        });
    }
}
