namespace DurableMultiAgentTemplate.Client.Components.Utilities;

/// <summary>
/// Context class for managing scroll-to-bottom operations in the UI.
/// Used to communicate scroll requests between components.
/// </summary>
public class ScrollToBottomContext
{
    /// <summary>
    /// Gets a value indicating whether a scroll to bottom operation has been requested.
    /// </summary>
    public bool IsRequestScrollToBottom { get; private set; }

    /// <summary>
    /// Requests a scroll to bottom operation.
    /// Sets the IsRequestScrollToBottom flag to true.
    /// </summary>
    public void RequestScrollToBottom()
    {
        IsRequestScrollToBottom = true;
    }

    /// <summary>
    /// Resets the scroll request.
    /// Sets the IsRequestScrollToBottom flag to false.
    /// </summary>
    public void Reset()
    {
        IsRequestScrollToBottom = false;
    }
}
