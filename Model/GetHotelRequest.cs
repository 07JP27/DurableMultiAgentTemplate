using OpenAI.Chat;

namespace DurableMultiAgentTemplate
{
    public class GetHotelRequest
    {
        public string Location { get; set; } = string.Empty;
    }
}