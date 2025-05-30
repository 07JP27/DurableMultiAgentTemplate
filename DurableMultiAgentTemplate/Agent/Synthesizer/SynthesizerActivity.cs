using DurableMultiAgentTemplate.Extension;
using DurableMultiAgentTemplate.Model;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using DurableMultiAgentTemplate.Shared.Model;

namespace DurableMultiAgentTemplate.Agent.Synthesizer;

/// <summary>
/// Activity class responsible for synthesizing results from multiple agent calls into a unified response.
/// Processes and formats agent outputs into coherent and user-friendly content.
/// </summary>
public class SynthesizerActivity(ChatClient chatClient, ILogger<SynthesizerActivity> logger)
{
    /// <summary>
    /// Synthesizes results from multiple agent calls into a coherent response.
    /// Uses OpenAI to combine and format agent outputs into user-friendly content.
    /// </summary>
    /// <param name="req">Request containing agent results, original request, and called agent names for synthesis</param>
    /// <returns>Unified agent response with synthesized content and list of called agents</returns>
    [Function(AgentActivityName.SynthesizerActivity)]
    public async Task<AgentResponseDto> Run([ActivityTrigger] SynthesizerRequest req)
    {
        logger.LogInformation("Run SynthesizerActivity");
        var systemMessageTemplate = SynthesizerPrompt.SystemPrompt;
        var systemMessage = $"{systemMessageTemplate}¥n{string.Join("¥n", req.AgentCallResult)}";

        ChatMessage[] allMessages = [
            new SystemChatMessage(systemMessage),
            .. req.AgentRequest.Messages.ConvertToChatMessageArray(),
        ];

        var chatResult = await chatClient.CompleteChatAsync(
            allMessages
        );

        if (chatResult.Value.FinishReason == ChatFinishReason.Stop)
        {
            return new AgentResponseDto(
                chatResult.Value.Content.First().Text,
                req.CalledAgentNames);
        }

        throw new InvalidOperationException("Failed to synthesize the result");
    }
}
