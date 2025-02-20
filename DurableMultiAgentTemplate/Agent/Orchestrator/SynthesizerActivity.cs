using Azure.AI.OpenAI;
using DurableMultiAgentTemplate.Extension;
using DurableMultiAgentTemplate.Model;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using DurableMultiAgentTemplate.Shared.Model;

namespace DurableMultiAgentTemplate.Agent.Orchestrator;

public class SynthesizerActivity(ChatClient chatClient, ILogger<SynthesizerActivity> logger)
{
    [Function(AgentActivityName.SynthesizerActivity)]
    public async Task<AgentResponseDto> Run([ActivityTrigger] SynthesizerRequest req, FunctionContext executionContext)
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
            return new AgentResponseDto
            {
                Content = chatResult.Value.Content.First().Text,
                CalledAgentNames = req.CalledAgentNames
            };
        }

        throw new InvalidOperationException("Failed to synthesize the result");
    }
}
