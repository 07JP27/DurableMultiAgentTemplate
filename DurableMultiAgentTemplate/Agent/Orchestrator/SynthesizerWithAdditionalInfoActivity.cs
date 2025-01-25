using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.AI.OpenAI;
using DurableMultiAgentTemplate.Extension;
using DurableMultiAgentTemplate.Json;
using DurableMultiAgentTemplate.Model;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace DurableMultiAgentTemplate.Agent.Orchestrator;

public class SynthesizerWithAdditionalInfoActivity(AzureOpenAIClient openAIClient, IOptions<AppConfiguration> configuration)
{
    private readonly AzureOpenAIClient _openAIClient = openAIClient;
    private readonly AppConfiguration _configuration = configuration.Value;

    [Function(AgentActivityName.SynthesizerWithAdditionalInfoActivity)]
    public async Task<AgentResponseWithAdditionalInfoDto> Run([ActivityTrigger] SynthesizerRequest req, FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("SynthesizerActivity");
        logger.LogInformation("Run SynthesizerActivity");
        var systemMessageTemplate = SynthesizerWithAdditionalInfoPrompt.SystemPrompt;
        var systemMessage = $"{systemMessageTemplate}¥n{string.Join("¥n", req.AgentCallResult)}";

        ChatMessage[] allMessages = [
            new SystemChatMessage(systemMessage),
            .. req.AgentReques.Messages.ConvertToChatMessageArray(),
        ];

        ChatCompletionOptions options = new()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
            "AgentResponseWithAdditionalInfo",
            JsonSchemaGenerator.GenerateSchemaAsBinaryData(SourceGenerationContext.Default.AgentResponseWithAdditionalInfoFormat))
        };

        var chatClient = _openAIClient.GetChatClient(_configuration.OpenAIDeploy);
        var chatResult = await chatClient.CompleteChatAsync(
            allMessages,
            options
        );

        if (chatResult.Value.FinishReason == ChatFinishReason.Stop)
        {
            AgentResponseWithAdditionalInfoFormat? res = JsonSerializer.Deserialize(
                chatResult.Value.Content.First().Text,
                SourceGenerationContext.Default.AgentResponseWithAdditionalInfoFormat);

            if (res == null) throw new InvalidOperationException("Failed to deserialize the result");

            return new AgentResponseWithAdditionalInfoDto
            {
                Content = res.Content ?? throw new InvalidOperationException("Content is null"),
                AdditionalInfo = res.AdditionalInfo ?? throw new InvalidOperationException("AdditionalInfo is null"),
                CalledAgentNames = req.CalledAgentNames,
            };
        }

        throw new InvalidOperationException("Failed to synthesize the result");
    }
}
