using OpenAI.Chat;
using DurableMultiAgentTemplate.Shared.Model;

namespace DurableMultiAgentTemplate.Extension;

public static class AgentRequestMessageItemExtension
{
    public static IEnumerable<ChatMessage> ConvertToChatMessageArray(this IEnumerable<MessageItem> messages)
    {
        return messages.Select<MessageItem, ChatMessage>(m =>
        {
            return m.Role switch
            {
                AgentRole.User => new UserChatMessage(m.Content),
                AgentRole.Agent => new AssistantChatMessage(m.Content),
                _ => throw new InvalidOperationException($"You can not set role: {m.Role}")
            };
        });
    }
}
