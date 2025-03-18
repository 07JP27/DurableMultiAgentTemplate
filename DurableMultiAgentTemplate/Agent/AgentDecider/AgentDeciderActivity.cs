using System.Text.Json;
using DurableMultiAgentTemplate.Extension;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using DurableMultiAgentTemplate.Shared.Model;
using DurableMultiAgentTemplate.Agent.Workers;
using System.Text.Json.Nodes;


namespace DurableMultiAgentTemplate.Agent.AgentDecider;

public class AgentDeciderActivity(ChatClient chatClient, AgentDefinitions agentDefinitions, ILogger<AgentDeciderActivity> logger)
{
    private static JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    [Function(AgentActivityNames.AgentDeciderActivity)]
    public async Task<AgentDeciderResult> Run([ActivityTrigger] AgentRequestDto reqData)
    {
        var messages = reqData.Messages.ConvertToChatMessageArray();
        var lastAgentMessage = (AgentMessageItem?)reqData.Messages.FindLast(x => x.Role == AgentRole.Agent);
        if (lastAgentMessage != null && lastAgentMessage.NextAgentCall != null)
        {
            var result = await DecideNextAgentCallAsync(messages, lastAgentMessage);
            if (result != null)
            {
                return result;
            }
        }

        logger.LogInformation("Run AgentDeciderActivity");

        ChatMessage[] allMessages = [
            new SystemChatMessage(AgentDeciderPrompt.SystemPrompt),
            .. messages,
        ];

        var chatResult = await chatClient.CompleteChatAsync(
            allMessages,
            CreateChatOptions(false)
        );

        if (chatResult.Value.FinishReason == ChatFinishReason.ToolCalls)
        {
            return new AgentDeciderResult(
                IsAgentCall: true,
                Content: "",
                AgentCalls: [.. chatResult.Value
                    .ToolCalls
                    .Select(toolCall => new AgentCall(
                        toolCall.FunctionName,
                        JsonSerializer.Deserialize<JsonElement>(toolCall.FunctionArguments)))
                ]
            );
        }
        else
        {
            if (chatResult.Value.FinishReason == ChatFinishReason.Stop)
            {
                return new AgentDeciderResult(
                    IsAgentCall: false,
                    Content: chatResult.Value.Content.First().Text,
                    AgentCalls: []
                );
            }
        }

        throw new InvalidOperationException("Invalid OpenAI response");
    }

    private async Task<AgentDeciderResult?> DecideNextAgentCallAsync(IEnumerable<ChatMessage> messages, AgentMessageItem lastAgentMessageItem)
    {
        var nextAgentCall = lastAgentMessageItem.NextAgentCall;
        if (nextAgentCall == null)
        {
            return null;
        }

        ChatMessage[] allMessagesForNextAgentCall = [
            new SystemChatMessage(AgentDeciderPrompt.SystemPromptForNextAgentCall),
                .. messages,
            ];
        var chatResultForNextAgentCall = await chatClient.CompleteChatAsync(
            allMessagesForNextAgentCall,
            CreateChatOptions(true)
        );

        if (chatResultForNextAgentCall.Value.FinishReason != ChatFinishReason.ToolCalls)
        {
            return null;
        }

        if (chatResultForNextAgentCall.Value.ToolCalls.Count != 1)
        {
            return null;
        }

        var toolCall = chatResultForNextAgentCall.Value.ToolCalls[0];
        if (toolCall.FunctionName != nextAgentCall.AgentName)
        {
            return null;
        }

        var argJsonElement = JsonSerializer.Deserialize<JsonElement>(toolCall.FunctionArguments);
        var argJson = JsonSerializer.Serialize(argJsonElement, _jsonSerializerOptions);
        var prevJson = JsonSerializer.Serialize(nextAgentCall.Arguments, _jsonSerializerOptions);
        if (argJson != prevJson)
        {
            return null;
        }

        return new AgentDeciderResult(
            IsAgentCall: true,
            Content: "",
            AgentCalls: [.. chatResultForNextAgentCall.Value
                .ToolCalls
                .Select(toolCall => new AgentCall(
                    toolCall.FunctionName,
                    argJsonElement))
            ]
        );
    }

    private ChatCompletionOptions CreateChatOptions(bool requiresUserConfirmation)
    {
        ChatCompletionOptions options = new();
        var agents = agentDefinitions.GetAgentDefinitions(requiresUserConfirmation);
        foreach (var agent in agents)
        {
            options.Tools.Add(agent.ChatTool);
        }

        return options;
    }
}
