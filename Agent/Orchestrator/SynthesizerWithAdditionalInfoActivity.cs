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
        var rawRes = chatResult.Value.Content.First();

        AgentResponseWithAdditionalInfoFormat res = JsonSerializer.Deserialize<AgentResponseWithAdditionalInfoFormat>(
            rawRes.Text, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return new AgentResponseWithAdditionalInfoDto
        {
            Content = res.Content,
            AdditionalInfo = res.AdditionalInfo,
            CalledAgentNames = req.CalledAgentNames,
        };
    }
}
