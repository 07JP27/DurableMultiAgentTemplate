namespace DurableMultiAgentTemplate.Client.Model;

/// <summary>
/// Represents user chat input in the client application.
/// Contains the message text and a flag indicating whether additional information is required.
/// </summary>
public class ChatInput
{
    /// <summary>
    /// Gets or sets a value indicating whether the response should include additional information.
    /// </summary>
    public bool RequireAdditionalInfo { get; set; }
    
    /// <summary>
    /// Gets or sets the message content entered by the user.
    /// </summary>
    public string Message { get; set; } = "";
}
