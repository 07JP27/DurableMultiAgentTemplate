using System.Text.Json;
using DurableMultiAgentTemplate.Extension;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using DurableMultiAgentTemplate.Shared.Model;


namespace DurableMultiAgentTemplate.Agent.AgentDecider;

public class AgentDeciderActivity(ChatClient chatClient, ILogger<AgentDeciderActivity> logger)
{
    [Function(AgentActivityName.AgentDeciderActivity)]
    public async Task<AgentDeciderResult> Run([ActivityTrigger] AgentRequestDto reqData)
    {
        var messages = reqData.Messages.ConvertToChatMessageArray();
        logger.LogInformation("Run AgentDeciderActivity");

        ChatMessage[] allMessages = [
            new SystemChatMessage(AgentDeciderPrompt.SystemPrompt),
            .. messages,
        ];
        ChatCompletionOptions options = new()
        {
            Tools = {
                AgentDefinition.GetDestinationSuggestAgent,
                AgentDefinition.GetClimateAgent,
                AgentDefinition.GetSightseeingSpotAgent,
                AgentDefinition.GetHotelAgent,
                AgentDefinition.SubmitReservationAgent
            }
        };

        var chatResult = await chatClient.CompleteChatAsync(
            allMessages,
            options
        );

        if (chatResult.Value.FinishReason == ChatFinishReason.ToolCalls)
        {
            return new AgentDeciderResult(
                IsAgentCall: true,
                Content: "",
                AgentCalls: [.. chatResult.Value
                    .ToolCalls
                    .Select(toolCall => new AgentCall(toolCall.FunctionName, JsonDocument.Parse(toolCall.FunctionArguments)))
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
}
