using Azure.AI.OpenAI;
using DurableMultiAgentTemplate.Model;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Options;

namespace DurableMultiAgentTemplate.Agent.SubmitReservationAgent;

public class SubmitReservationActivity(AzureOpenAIClient openAIClient, IOptions<AppConfiguration> configuration)
{
    private readonly AzureOpenAIClient _openAIClient = openAIClient;
    private readonly AppConfiguration _configuration = configuration.Value;

    [Function(AgentActivityName.SubmitReservationAgent)]
    public string Run([ActivityTrigger] SubmitReservationRequest req, FunctionContext executionContext)
    {
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
