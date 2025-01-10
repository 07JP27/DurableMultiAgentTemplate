using System.Text.Json.Serialization;

namespace DurableMultiAgentTemplate
{
    public class AgentRequestDto
    {
        [JsonPropertyName("messages")]
        public List<AgentRequestMessageItem> Messages { get; set; }= default!;
    }

    public class AgentRequestMessageItem
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;
        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }
}