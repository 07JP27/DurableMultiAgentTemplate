using Azure.AI.OpenAI;
using DurableMultiAgentTemplate.Model;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace DurableMultiAgentTemplate.Agent.SubmitReservationAgent;

public class SubmitReservationActivity(ChatClient chatClient)//, CosmosClient cosmosClient)
{
    [Function(AgentActivityName.SubmitReservationAgent)]
    public async Task<string> RunAsync([ActivityTrigger] SubmitReservationRequest req)
    {
        // Simulate a delay
        await Task.Delay(3000);

        // This is sample code. Replace this with your own logic.
        var result = $"""
        予約番号は {Guid.NewGuid()} です。
        --------------------------------
        ホテル名：{req.Destination}
        チェックイン日：{req.CheckIn}
        チェックアウト日：{req.CheckOut}
        人数：{req.GuestsCount} 名
        --------------------------------
        """;

        return result;
    }
}
