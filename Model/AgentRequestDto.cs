using OpenAI.Chat;

namespace DurableMultiAgentTemplate
{
    public class AgentRequestDto
    {
        public List<AgentRequestMessageItem> Messages { get; set; }= default!;

        public ChatMessage[] ConvertToChatMessageArray()
        {
            return Messages.Select<AgentRequestMessageItem, ChatMessage>(m =>
            {
                return m.Role switch
                {
                    "system" => new SystemChatMessage (m.Content),
                    "user" => new UserChatMessage (m.Content),
                    "assistant" => new AssistantChatMessage (m.Content),
                    _ => throw new InvalidOperationException($"Unknown role: {m.Role}")
                };
            }).ToArray();
        }
    }

    public class AgentRequestMessageItem
    {
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}