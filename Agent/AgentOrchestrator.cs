using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;

namespace DurableMultiAgentTemplate
{
    public class AgentOrchestrator
    {
        private readonly AzureOpenAIClient _openAIClient;
        private readonly AppConfiguration _configuration;

        public AgentOrchestrator(AzureOpenAIClient openAIClient, AppConfiguration configuration)
        {
            _openAIClient = openAIClient;
            _configuration = configuration;
        }

        [Function(nameof(AgentOrchestrator))]
        public async Task<AgentResponseDto> RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            ILogger logger = context.CreateReplaySafeLogger("AgentOrchestrator");
            var reqData = context.GetInput<AgentRequestDto>();
            
            AgentResponseDto response = new AgentResponseDto
            {
                Content = await context.CallActivityAsync<string>(nameof(AskChatGPT), reqData.Messages.First().Content)
            };

            return response;
        }

        [Function(nameof(AskChatGPT))]
        public async Task<string> AskChatGPT([ActivityTrigger] string prompt, FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("SayHello");
            logger.LogInformation("Saying hello to {name}.", prompt);
             var chatClient = _openAIClient.GetChatClient(_configuration.OpenAIDeploy);
            logger.LogInformation($"{_openAIClient}");
            var chatResult = await chatClient.CompleteChatAsync(
                new UserChatMessage(prompt)
            );

            return chatResult.Value.Content.First().Text;
        }
    }
}
