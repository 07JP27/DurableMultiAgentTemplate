using System.Text.Json;
using DurableMultiAgentTemplate.Extension;
using DurableMultiAgentTemplate.Json;
using DurableMultiAgentTemplate.Model;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using DurableMultiAgentTemplate.Shared.Model;


namespace DurableMultiAgentTemplate.Agent.Synthesizer;

/// <summary>
/// Activity class responsible for synthesizing results from multiple agent calls into a unified response
/// with additional information. Handles formatting and processing responses with supplementary data.
/// </summary>
public class SynthesizerWithAdditionalInfoActivity(ChatClient chatClient, ILogger<SynthesizerWithAdditionalInfoActivity> logger)
{
    /// <summary>
    /// Synthesizes results from multiple agent calls into a unified response with additional information.
    /// Uses OpenAI structured output to combine agent results and generate supplementary data.
    /// </summary>
    /// <param name="req">Request containing agent results, original request, and called agent names for synthesis</param>
    /// <returns>Enhanced agent response with synthesized content, additional information, and list of called agents</returns>
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
