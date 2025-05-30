using Microsoft.Azure.Functions.Worker;
using OpenAI.Chat;

namespace DurableMultiAgentTemplate.Agent.SubmitReservationAgent;

/// <summary>
/// Activity class responsible for handling hotel reservation submissions.
/// Creates a reservation record and returns confirmation details.
/// </summary>
public class SubmitReservationActivity(ChatClient chatClient)//, CosmosClient cosmosClient)
{
    /// <summary>
    /// Processes a hotel reservation request and generates a confirmation.
    /// Simulates reservation processing and returns booking details with a unique reservation number.
    /// </summary>
    /// <param name="req">Request containing reservation details including destination, dates, and guest count</param>
    /// <returns>Reservation confirmation with booking details and reservation number in Japanese</returns>
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
