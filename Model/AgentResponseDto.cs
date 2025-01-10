using System.Text.Json.Serialization;

namespace DurableMultiAgentTemplate.Model
{
    public class AgentResponseDto
    {
        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        [JsonPropertyName("caledAgentNames")]
        public List<string> CaledAgentNames { get; set; } = new List<string>();

    }
}