using System.Text.Json;
using DurableMultiAgentTemplate.Agent.Orchestrator;
using DurableMultiAgentTemplate.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace DurableMultiAgentTemplate;

public class Starter(ILogger<Starter> logger)
{
    private readonly ILogger<Starter> _logger = logger;

    [Function("SyncStarter")]
    public async Task<HttpResponseData> SyncStarter(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "invoke/sync")] HttpRequestData req,
        [DurableClient] DurableTaskClient client)
    {
        _logger.LogInformation("Sync HTTP trigger function processed a request.");
        var reqData = await GetRequestData(req);

        if (reqData == null)
        {
            var badRequestResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
            await badRequestResponse.WriteStringAsync("Please pass a prompt in the request body");
            return badRequestResponse;
        }

        string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(AgentOrchestrator), reqData);

        _logger.LogInformation($"Started orchestration with ID = '{instanceId}'.");

        OrchestrationMetadata metadata = await client.WaitForInstanceCompletionAsync(instanceId, getInputsAndOutputs: true);

        var res = HttpResponseData.CreateResponse(req);
        await res.WriteStringAsync(metadata.SerializedOutput ?? "");
        return res;
    }

    [Function("AsyncStarter")]
    public async Task<HttpResponseData> AsyncStarter(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "invoke/async")] HttpRequestData req,
        [DurableClient] DurableTaskClient client)
    {
        _logger.LogInformation("Async HTTP trigger function processed a request.");
        var reqData = await GetRequestData(req);

        if (reqData == null)
        {
            var badRequestResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
            await badRequestResponse.WriteStringAsync("Please pass a prompt in the request body");
            return badRequestResponse;
        }

        string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(AgentOrchestrator), reqData);

        _logger.LogInformation($"Started orchestration with ID = '{instanceId}'.");

        return await client.CreateCheckStatusResponseAsync(req, instanceId);
    }

    private async Task<AgentRequestDto?> GetRequestData(HttpRequestData req)
    {
        var requestBody = await req.ReadAsStringAsync();

        if (string.IsNullOrEmpty(requestBody)) return null;

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var reqData = JsonSerializer.Deserialize<AgentRequestDto>(requestBody, options);

        if (reqData == null) return null;
        if (reqData.Messages == null || !reqData.Messages.Any()) return null;
        return reqData;
    }
}
