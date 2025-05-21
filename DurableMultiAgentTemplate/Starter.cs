using System.Text.Json;
using DurableMultiAgentTemplate.Agent.Orchestrator;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using DurableMultiAgentTemplate.Shared.Model;

namespace DurableMultiAgentTemplate;

/// <summary>
/// Class providing starter functions for agent orchestration.
/// Provides synchronous and asynchronous HTTP triggers to start agent orchestration.
/// </summary>
public class Starter(ILogger<Starter> logger)
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Synchronously executes agent orchestration and waits for completion. HTTP trigger function.
    /// </summary>
    /// <param name="req">HTTP request data</param>
    /// <param name="client">Durable task client</param>
    /// <returns>HTTP response containing agent processing results</returns>
    [Function("SyncStarter")]
    public async Task<HttpResponseData> SyncStarter(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "invoke/sync")] HttpRequestData req,
        [DurableClient] DurableTaskClient client)
    {
        logger.LogInformation("Sync HTTP trigger function processed a request.");
        var reqData = await GetRequestData(req);

        if (reqData == null)
        {
            var badRequestResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
            await badRequestResponse.WriteStringAsync("Please pass a prompt in the request body");
            return badRequestResponse;
        }

        var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(AgentOrchestrator), reqData);

        logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

        var metadata = await client.WaitForInstanceCompletionAsync(instanceId, getInputsAndOutputs: true);

        var res = HttpResponseData.CreateResponse(req);
        await res.WriteStringAsync(metadata.SerializedOutput ?? "");
        return res;
    }

    /// <summary>
    /// Asynchronously starts agent orchestration. HTTP trigger function.
    /// </summary>
    /// <param name="req">HTTP request data</param>
    /// <param name="client">Durable task client</param>
    /// <returns>HTTP response containing URLs for checking orchestration status</returns>
    [Function("AsyncStarter")]
    public async Task<HttpResponseData> AsyncStarter(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "invoke/async")] HttpRequestData req,
        [DurableClient] DurableTaskClient client)
    {
        logger.LogInformation("Async HTTP trigger function processed a request.");
        var reqData = await GetRequestData(req);

        if (reqData == null)
        {
            var badRequestResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
            await badRequestResponse.WriteStringAsync("Please pass a prompt in the request body");
            return badRequestResponse;
        }

        var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(AgentOrchestrator), reqData);

        logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

        return await client.CreateCheckStatusResponseAsync(req, instanceId);
    }

    /// <summary>
    /// Helper method to retrieve agent request data from an HTTP request.
    /// </summary>
    /// <param name="req">HTTP request data</param>
    /// <returns>Agent request data, or null if the request is invalid</returns>
    private async Task<AgentRequestDto?> GetRequestData(HttpRequestData req)
    {
        var requestBody = await req.ReadAsStringAsync();

        if (string.IsNullOrEmpty(requestBody)) return null;

        var reqData = JsonSerializer.Deserialize<AgentRequestDto>(requestBody, _jsonSerializerOptions);

        if (reqData == null) return null;
        if (reqData.Messages == null || reqData.Messages.Count == 0) return null;
        return reqData;
    }
}
