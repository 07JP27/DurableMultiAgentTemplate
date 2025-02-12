using OpenAI.Chat;
using DurableMultiAgentTemplate.Shared.Model;

namespace DurableMultiAgentTemplate.Extension;

public static class AgentRequestMessageItemExtension
{
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
