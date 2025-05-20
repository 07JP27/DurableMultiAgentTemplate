using System.Text.Json;
using DurableMultiAgentTemplate.Agent.Orchestrator;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using DurableMultiAgentTemplate.Shared.Model;

namespace DurableMultiAgentTemplate;

/// <summary>
/// エージェントオーケストレーションを開始するスターター機能を提供するクラス。
/// 同期および非同期のHTTPトリガーを提供し、エージェントのオーケストレーションを開始します。
/// </summary>
public class Starter(ILogger<Starter> logger)
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// 同期的にエージェントオーケストレーションを実行し、完了を待機するHTTPトリガー関数。
    /// </summary>
    /// <param name="req">HTTPリクエストデータ</param>
    /// <param name="client">Durableタスククライアント</param>
    /// <returns>エージェントの処理結果を含むHTTPレスポンス</returns>
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
    /// 非同期的にエージェントオーケストレーションを開始するHTTPトリガー関数。
    /// </summary>
    /// <param name="req">HTTPリクエストデータ</param>
    /// <param name="client">Durableタスククライアント</param>
    /// <returns>オーケストレーションの状態確認用URLを含むHTTPレスポンス</returns>
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
    /// HTTPリクエストからエージェントリクエストデータを取得する補助メソッド。
    /// </summary>
    /// <param name="req">HTTPリクエストデータ</param>
    /// <returns>エージェントリクエストデータ、またはリクエストが無効な場合はnull</returns>
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
