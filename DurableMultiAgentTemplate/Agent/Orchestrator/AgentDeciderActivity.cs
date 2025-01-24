using System.Text.Json;
using Azure.AI.OpenAI;
using DurableMultiAgentTemplate.Extension;
using DurableMultiAgentTemplate.Model;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI.Chat;


namespace DurableMultiAgentTemplate.Agent.Orchestrator;

public class AgentDeciderActivity(AzureOpenAIClient openAIClient, IOptions<AppConfiguration> configuration)
{
    private readonly AzureOpenAIClient _openAIClient = openAIClient;
    private readonly AppConfiguration _configuration = configuration.Value;

    [Function(AgentActivityName.AgentDeciderActivity)]
    public async Task<AgentDeciderResult> Run([ActivityTrigger] AgentRequestDto reqData, FunctionContext executionContext)
    {
        var messages = reqData.Messages.ConvertToChatMessageArray();
        ILogger logger = executionContext.GetLogger("AgentDeciderActivity");
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

        var chatClient = _openAIClient.GetChatClient(_configuration.OpenAIDeploy);
        var chatResult = await chatClient.CompleteChatAsync(
            allMessages,
            options
        );

        if (chatResult.Value.FinishReason == ChatFinishReason.ToolCalls)
        {
            return new AgentDeciderResult
            {
                IsAgentCall = true,
                AgentCalls = chatResult.Value.ToolCalls.Select(toolCall => new AgentCall
                {
                    AgentName = toolCall.FunctionName,
                    Arguments = JsonDocument.Parse(toolCall.FunctionArguments)
                }).ToArray()
            };
        }
        else
        {
            if (chatResult.Value.FinishReason == ChatFinishReason.Stop)
            {
                return new AgentDeciderResult
                {
                    IsAgentCall = false,
                    Content = chatResult.Value.Content.First().Text
                };
            }
        }
        
        throw new InvalidOperationException("Invalid OpenAI response");
    }
}
