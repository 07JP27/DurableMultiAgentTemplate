using DurableMultiAgentTemplate.Client.Components.Utilities;
using DurableMultiAgentTemplate.Client.Model;
using DurableMultiAgentTemplate.Client.Services;
using DurableMultiAgentTemplate.Client.Utilities;
using DurableMultiAgentTemplate.Model;
using Microsoft.FluentUI.AspNetCore.Components;

namespace DurableMultiAgentTemplate.Client.Components.Pages;

public partial class Home(AgentChatService agentChatService, ILogger<Home> logger)
{
    private readonly ScrollToBottomContext _scrollToBottomContext = new();
    private readonly ExecutionTracker _executionTracker = new();
    private readonly ChatInput _chatInput = new();
    private readonly List<ChatMessage> _messages = [];
    private readonly List<IAdditionalInfo> _additionalInfo = [];

    private async Task SendMessageAsync()
    {
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
            var response = await agentChatService.GetAgentResponseAsync(new AgentRequestDto
            {
                Messages = _messages.Where(x => x.IsRequestTarget).Select(x => x switch
                {
                    UserChatMessage userChatMessage => new AgentRequestMessageItem
                    {
                        Role = userChatMessage.Role.ToRoleName(),
                        Content = userChatMessage.Message,
                    },
                    AgentChatMessage agentChatMessage => new AgentRequestMessageItem
                    {
                        Role = agentChatMessage.Role.ToRoleName(),
                        Content = agentChatMessage.Message.Content,
                    },
                    _ => throw new InvalidOperationException()
                })
                .ToList(),
                RequireAdditionalInfo = _chatInput.RequireAdditionalInfo,
            });
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
            _chatInput.Message = message;
            _messages.RemoveAt(_messages.Count - 1);
            _messages.Add(new InfoChatMessage($"Failed to send message: {e.Message}", false));
            _scrollToBottomContext.RequestScrollToBottom();
        }
    }

    private void Reset()
    {
        _messages.Clear();
        _additionalInfo.Clear();
    }
}
