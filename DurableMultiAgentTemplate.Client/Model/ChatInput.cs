namespace DurableMultiAgentTemplate.Client.Model;

public class ChatInput
{
    public bool RequireAdditionalInfo { get; set; }
    public string Message { get; set; } = "";
}
