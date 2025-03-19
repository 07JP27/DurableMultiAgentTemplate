using DurableMultiAgentTemplate.Extension;
using DurableMultiAgentTemplate.Model;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using DurableMultiAgentTemplate.Shared.Model;

namespace DurableMultiAgentTemplate.Agent.Synthesizer;

public class SynthesizerActivity(ChatClient chatClient, ILogger<SynthesizerActivity> logger)
{
    [Function(AgentActivityNames.SynthesizerActivity)]
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
            var nextAgentCall = req.AgentCallResult
                .Select(x => x.NextAgentCall)
                .SingleOrDefault(x => x != null);
            return new(
                new(chatResult.Value.Content.First().Text, 
                    nextAgentCall),
                req.CalledAgentNames);
        }

        throw new InvalidOperationException("Failed to synthesize the result");
    }
}
