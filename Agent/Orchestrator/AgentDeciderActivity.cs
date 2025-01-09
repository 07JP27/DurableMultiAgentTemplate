using System;
using System.Text.Json;
using Azure.AI.OpenAI;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;


namespace DurableMultiAgentTemplate
{
    public class AgentDeciderActivity(AzureOpenAIClient openAIClient, AppConfiguration configuration)
    {
        private readonly AzureOpenAIClient _openAIClient = openAIClient;
        private readonly AppConfiguration _configuration = configuration;

        [Function(AgentActivityName.AgentDeciderActivity)]
        public async Task<AgentDeciderResult> Run([ActivityTrigger] AgentRequestDto reqData, FunctionContext executionContext)
        {
            var messages = reqData.ConvertToChatMessageArray();
            ILogger logger = executionContext.GetLogger("AgentDeciderActivity");
            logger.LogInformation("Run AgentDeciderActivity");

            ChatMessage[] systemMessage = {new SystemChatMessage(AgentDeciderPrompt.SystemPrompt)};
            var allMessages = systemMessage.Concat(messages).ToArray();
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
                var result = new AgentDeciderResult
                {
                    IsAgentCall = true,
                    AgentCalls = chatResult.Value.ToolCalls.Select(toolCall => new AgentCall
                    {
                        AgentName = toolCall.FunctionName,
                        Arguments = JsonDocument.Parse(toolCall.FunctionArguments)
                    }).ToArray()
                };
                return result;
            }
            else
            {
                return new AgentDeciderResult
                {
                    IsAgentCall = false,
                    Content = chatResult.Value.Content.First().Text
                };
            }
        }
    }
}
