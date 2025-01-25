using Azure.AI.OpenAI;
using DurableMultiAgentTemplate.Extension;
using DurableMultiAgentTemplate.Model;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace DurableMultiAgentTemplate.Agent.Orchestrator;

public class SynthesizerActivity(AzureOpenAIClient openAIClient, IOptions<AppConfiguration> configuration)
{
    private readonly AzureOpenAIClient _openAIClient = openAIClient;
    private readonly AppConfiguration _configuration = configuration.Value;

    [Function(AgentActivityName.SynthesizerActivity)]
    public async Task<AgentResponseDto> Run([ActivityTrigger] SynthesizerRequest req, FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("SynthesizerActivity");
        logger.LogInformation("Run SynthesizerActivity");
        var systemMessageTemplate = SynthesizerPrompt.SystemPrompt;
        var systemMessage = $"{systemMessageTemplate}¥n{string.Join("¥n", req.AgentCallResult)}";

        ChatMessage[] allMessages = [
            new SystemChatMessage(systemMessage),
            .. req.AgentReques.Messages.ConvertToChatMessageArray(),
        ];

        var chatClient = _openAIClient.GetChatClient(_configuration.OpenAIDeploy);
        var chatResult = await chatClient.CompleteChatAsync(
            allMessages
        );

        if (chatResult.Value.FinishReason == ChatFinishReason.Stop)
        {
            return new AgentResponseDto
            {
                Content = chatResult.Value.Content.First().Text,
                CalledAgentNames = req.CalledAgentNames
            };
        }

        throw new InvalidOperationException("Failed to synthesize the result");
    }
}
