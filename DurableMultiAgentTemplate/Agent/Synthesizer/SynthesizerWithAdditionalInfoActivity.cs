using System.Text.Json;
using DurableMultiAgentTemplate.Extension;
using DurableMultiAgentTemplate.Json;
using DurableMultiAgentTemplate.Model;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using DurableMultiAgentTemplate.Shared.Model;


namespace DurableMultiAgentTemplate.Agent.Synthesizer;

public class SynthesizerWithAdditionalInfoActivity(ChatClient chatClient, ILogger<SynthesizerWithAdditionalInfoActivity> logger)
{
    [Function(AgentActivityName.SynthesizerWithAdditionalInfoActivity)]
    public async Task<AgentResponseWithAdditionalInfoDto> Run([ActivityTrigger] SynthesizerRequest req)
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
            var res = JsonSerializer.Deserialize(
                chatResult.Value.Content.First().Text,
                SourceGenerationContext.Default.AgentResponseWithAdditionalInfoFormat) ?? 
                throw new InvalidOperationException("Failed to deserialize the result");

            return new AgentResponseWithAdditionalInfoDto(
                res.Content ?? throw new InvalidOperationException("Content is null"),
                req.CalledAgentNames,
                res.AdditionalInfo ?? throw new InvalidOperationException("AdditionalInfo is null"));
        }

        throw new InvalidOperationException("Failed to synthesize the result");
    }
}
