using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DurableMultiAgentTemplate.Shared.Model;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;

namespace DurableMultiAgentTemplate.Agent.Workers.HumanInTheLoopAgent;

public class HumanInTheLoopActivity(ILogger<HumanInTheLoopActivity> logger,
    ChatClient chatClient)
{
    [Function(nameof(HumanInTheLoopActivity))]
    public async Task<string> RunAsync([ActivityTrigger] HumanInTheLoopRequest req)
    {
        logger.LogInformation("HumanInTheLoopActivity called");
        ChatMessage[] allMessages = [
            new SystemChatMessage(HumanInTheLoopPrompt.SystemPrompt),
            new UserChatMessage(req.UserRequest),
        ];

        var chatResult = await chatClient.CompleteChatAsync(
            allMessages
        );

        if (chatResult.Value.FinishReason == ChatFinishReason.Stop)
        {
            return chatResult.Value.Content.First().Text;
        }

        logger.LogError("Failed to generate a confirmation message for the user, the finish reason is {finishReason}.", chatResult.Value.FinishReason);
        throw new InvalidOperationException("Failed to generate a confirmation message for the user");
    }
}
