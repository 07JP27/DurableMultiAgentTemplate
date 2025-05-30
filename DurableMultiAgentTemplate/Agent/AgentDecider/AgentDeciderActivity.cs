using System.Text.Json;
using DurableMultiAgentTemplate.Extension;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using DurableMultiAgentTemplate.Shared.Model;


namespace DurableMultiAgentTemplate.Agent.AgentDecider;

/// <summary>
/// Activity class responsible for determining which agent(s) should be called based on user request.
/// Analyzes user input and decides appropriate agent routing for the multi-agent system.
/// </summary>
public class AgentDeciderActivity(ChatClient chatClient, ILogger<AgentDeciderActivity> logger)
{
    /// <summary>
    /// Executes the agent decision logic to determine which agents should be called.
    /// Analyzes user messages and uses OpenAI function calling to route to appropriate agents.
    /// </summary>
    /// <param name="reqData">Request data containing user messages and configuration</param>
    /// <returns>Agent decision result indicating whether agent calls are needed and which agents to call</returns>
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
