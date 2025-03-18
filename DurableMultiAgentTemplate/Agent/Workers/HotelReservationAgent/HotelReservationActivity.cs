using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using OpenAI.Chat;

namespace DurableMultiAgentTemplate.Agent.Workers.HotelReservationAgent;

public class HotelReservationActivity(ChatClient chatClient)//, CosmosClient cosmosClient)
{
    private static JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    [Function(AgentActivityNames.SubmitReservationAgent)]
    public async Task<WorkerAgentResult> SubmitReservationRequestAsync([ActivityTrigger] HotelReservationRequest req)
    {
        // Simulate a delay
        await Task.Delay(3000);

        return new(
            $"""
            以下の内容で予約を作成してもよろしいでしょうか？
            --------------------------------
            ホテル名：{req.Destination}
            チェックイン日：{req.CheckIn}
            チェックアウト日：{req.CheckOut}
            人数：{req.GuestsCount} 名
            --------------------------------
            """,
            new(AgentActivityNames.CommitReservationAgent, JsonSerializer.SerializeToElement(req, _jsonSerializerOptions)));
    }

    [Function(AgentActivityNames.CommitReservationAgent)]
    public async Task<WorkerAgentResult> CommitReservationAsync([ActivityTrigger] HotelReservationRequest req)
    {
        // Simulate a delay
        await Task.Delay(3000);

        // This is sample code. Replace this with your own logic.
        var result = $"""
            予約を行いました。予約番号は {Guid.NewGuid()} です。
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
