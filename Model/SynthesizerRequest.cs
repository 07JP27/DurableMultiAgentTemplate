using OpenAI.Chat;

namespace DurableMultiAgentTemplate
{
    public class SynthesizerRequest
    {
        public List<string> AgentCallResult { get; set; } = new List<string>();
        public AgentRequestDto AgentReques { get; set; } = new AgentRequestDto();
    }
}