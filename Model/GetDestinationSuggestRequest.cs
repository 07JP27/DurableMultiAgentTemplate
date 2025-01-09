using OpenAI.Chat;

namespace DurableMultiAgentTemplate
{
    public class GetDestinationSuggestRequest
    {
        public string SearchTerm { get; set; } = string.Empty;
    }
}