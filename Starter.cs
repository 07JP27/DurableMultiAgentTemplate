using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DurableTask.Client;
using System.Text.Json;

namespace DurableMultiAgentTemplate
{
    public class Starter
    {
        private readonly ILogger<Starter> _logger;

        public Starter(ILogger<Starter> logger)
        {
            _logger = logger;
        }

        [Function("SyncStarter")]
         public async Task<HttpResponseData> SyncStarter(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route="invoke/sync")] HttpRequestData req,
            [DurableClient] DurableTaskClient client)
        {
            _logger.LogInformation("Sync HTTP trigger function processed a request.");
            var prompt = await GetPrompt(req);

            if (prompt == null)
            {
                var badRequestResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                await badRequestResponse.WriteStringAsync("Please pass a prompt in the request body");
                return badRequestResponse;
            }
            
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(AgentOrchestrator), prompt);

            _logger.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            OrchestrationMetadata metadata = await client.WaitForInstanceCompletionAsync(instanceId, getInputsAndOutputs: true);

            var res = HttpResponseData.CreateResponse(req);
            await res.WriteAsJsonAsync(metadata.SerializedOutput);
            return res;
        }

        [Function("AsyncStarter")]
        public async Task<HttpResponseData> AsyncStarter(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route="invoke/async")] HttpRequestData req,
            [DurableClient] DurableTaskClient client)
        {
            _logger.LogInformation("Async HTTP trigger function processed a request.");
            var prompt = await GetPrompt(req);

            if (prompt == null)
            {
                var badRequestResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                await badRequestResponse.WriteStringAsync("Please pass a prompt in the request body");
                return badRequestResponse;
            }
            
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(AgentOrchestrator), prompt);

            _logger.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return await client.CreateCheckStatusResponseAsync(req, instanceId);
        }

        private async Task<List<AgentRequestDto>?> GetPrompt(HttpRequestData req)
        {
            var requestBody = await req.ReadAsStringAsync();
            if (string.IsNullOrEmpty(requestBody)) return null;
            var prompt = JsonSerializer.Deserialize<List<AgentRequestDto>>(requestBody);
            return prompt;
        }
    }
}
