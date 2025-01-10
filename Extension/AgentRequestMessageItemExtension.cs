using DurableMultiAgentTemplate.Model;
using OpenAI.Chat;

namespace DurableMultiAgentTemplate.Extension;

public static class AgentRequestMessageItemExtension
{
    public static ChatMessage[] ConvertToChatMessageArray(this List<AgentRequestMessageItem> messages)
    {
        return messages.Select<AgentRequestMessageItem, ChatMessage>(m =>
        {
            return m.Role switch
            {
                "system" => new SystemChatMessage(m.Content),
                "user" => new UserChatMessage(m.Content),
                "assistant" => new AssistantChatMessage(m.Content),
                _ => throw new InvalidOperationException($"Unknown role: {m.Role}")
            };
        }).ToArray();
    }
}