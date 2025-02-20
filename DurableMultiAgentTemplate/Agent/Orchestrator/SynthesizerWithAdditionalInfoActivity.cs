using System.Text.Json;
using DurableMultiAgentTemplate.Extension;
using DurableMultiAgentTemplate.Json;
using DurableMultiAgentTemplate.Model;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using DurableMultiAgentTemplate.Shared.Model;


namespace DurableMultiAgentTemplate.Agent.Orchestrator;

public class SynthesizerWithAdditionalInfoActivity(ChatClient chatClient, ILogger<SynthesizerWithAdditionalInfoActivity> logger)
{
    [Function(AgentActivityName.SynthesizerWithAdditionalInfoActivity)]
    public async Task<AgentResponseWithAdditionalInfoDto> Run([ActivityTrigger] SynthesizerRequest req, FunctionContext executionContext)
    {
        logger.LogInformation("Run SynthesizerActivity");
        var systemMessageTemplate = SynthesizerWithAdditionalInfoPrompt.SystemPrompt;
        var systemMessage = $"{systemMessageTemplate}¥n{string.Join("¥n", req.AgentCallResult)}";

        ChatMessage[] allMessages = [
            new SystemChatMessage(systemMessage),
            .. req.AgentRequest.Messages.ConvertToChatMessageArray(),
        ];

        ChatCompletionOptions options = new()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
            "AgentResponseWithAdditionalInfo",
            JsonSchemaGenerator.GenerateSchemaAsBinaryData(SourceGenerationContext.Default.AgentResponseWithAdditionalInfoFormat))
        };

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
