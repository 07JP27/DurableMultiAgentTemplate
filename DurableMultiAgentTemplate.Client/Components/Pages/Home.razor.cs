using DurableMultiAgentTemplate.Client.Components.Utilities;
using DurableMultiAgentTemplate.Client.Model;
using DurableMultiAgentTemplate.Client.Services;
using DurableMultiAgentTemplate.Client.Utilities;
using DurableMultiAgentTemplate.Shared.Model;


namespace DurableMultiAgentTemplate.Client.Components.Pages;

/// <summary>
/// Main home page component for the chat interface.
/// Handles user interactions and communication with agent services.
/// </summary>
public partial class Home(AgentChatService agentChatService, ILogger<Home> logger)
{
    private const int _chatTimeoutMs = 60000;

    private readonly ScrollToBottomContext _scrollToBottomContext = new();
    private readonly ExecutionTracker _executionTracker = new();
    private readonly ChatInput _chatInput = new();
    private readonly List<ChatMessage> _messages = [];
    private readonly List<IAdditionalInfo> _additionalInfo = [];

    /// <summary>
    /// Sends a message to the agent and processes the response.
    /// Manages the UI state during the request/response cycle.
    /// </summary>
    private async Task SendMessageAsync()
    {
        void handleError(string originalInputMessage, string errorMessage)
        {
            _chatInput.Message = originalInputMessage;
            _messages.RemoveAt(_messages.Count - 1);
            _messages.Add(new InfoChatMessage($"Failed to send message: {errorMessage}", false));
            _scrollToBottomContext.RequestScrollToBottom();
        }

        if (_executionTracker.IsInProgress) return;
        using var _ = _executionTracker.Start();
        if (string.IsNullOrWhiteSpace(_chatInput.Message)) return;

        var message = _chatInput.Message;
        _chatInput.Message = "";
        _messages.Add(new UserChatMessage(message));
        _messages.Add(new InfoChatMessage("Waiting for agent response...", true));
        _scrollToBottomContext.RequestScrollToBottom();

        try
        {
            // Send the message to the agent
            using CancellationTokenSource cancellationTokenSource = new();
            var progress = new Progress<AgentOrchestratorStatus>(status =>
            {
                static string createStatusMessage(AgentOrchestratorStatus status)
                {
                    return status.Step switch
                    {
                        AgentOrchestratorStep.AgentDeciderActivity => "Agent is deciding the next action...",
                        AgentOrchestratorStep.WorkerAgentActivity => $"Processing the task with {string.Join(", ", status.AgentCalls.Select(x => x.AgentName))}...",
                        AgentOrchestratorStep.SynthesizerActivity => "Synthesizing the final response...",
                        _ => "Waiting for agent response...",
                    };
                }

                _messages.RemoveAt(_messages.Count - 1);
                _messages.Add(new InfoChatMessage(createStatusMessage(status), true));
                _scrollToBottomContext.RequestScrollToBottom();
                StateHasChanged();
            });

            var getAgentResponseTask = agentChatService.GetAgentResponseAsync(new AgentRequestDto(
                Messages: [.. _messages.Where(x => x.IsRequestTarget).Select(x => x switch
                {
                    UserChatMessage userChatMessage => new AgentRequestMessageItem(
                        userChatMessage.Role.ToRoleName(),
                        userChatMessage.Message),
                    AgentChatMessage agentChatMessage => new AgentRequestMessageItem(
                        agentChatMessage.Role.ToRoleName(),
                        agentChatMessage.Message.Content),
                    _ => throw new InvalidOperationException()
                })],
                RequireAdditionalInfo: _chatInput.RequireAdditionalInfo), 
            progress,
            cancellationTokenSource.Token);

            // Wait for the agent response or timeout
            var winner = await Task.WhenAny(getAgentResponseTask, Task.Delay(_chatTimeoutMs));
            if (winner != getAgentResponseTask)
            {
                // Timeout
                cancellationTokenSource.Cancel();
                handleError(message,"The request timed out. Please try again.");
                return;
            }

            // Process the agent response
            var response = await getAgentResponseTask;
            _messages.RemoveAt(_messages.Count - 1);
            _messages.Add(new AgentChatMessage(response));

            if (response.AdditionalInfo is not null)
            {
                _additionalInfo.AddRange(response.AdditionalInfo);
            }

            _scrollToBottomContext.RequestScrollToBottom();
            _chatInput.Message = "";
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to send message: {Message}", message);
            handleError(message,$"Failed to send message: {e.Message}");
        }
    }

    /// <summary>
    /// Resets the chat interface by clearing all messages and additional information.
    /// </summary>
    private void Reset()
    {
        _messages.Clear();
        _additionalInfo.Clear();
    }
}
