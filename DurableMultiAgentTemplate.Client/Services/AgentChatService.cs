using System.Text.Json;
using DurableMultiAgentTemplate.Shared.Model;

namespace DurableMultiAgentTemplate.Client.Services;

public class AgentChatService(HttpClient httpClient)
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public async Task<AgentResponseWithAdditionalInfoDto> GetAgentResponseAsync(AgentRequestDto agentRequestDto, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/invoke/async", agentRequestDto, cancellationToken);
        response.EnsureSuccessStatusCode();

        var invokeAsyncResult = await response.Content.ReadFromJsonAsync<InvokeAsyncResult>(_jsonSerializerOptions, cancellationToken);
        if (invokeAsyncResult == null)
        {
            throw new InvalidOperationException("The format of the response from the agent is invalid.");
        }

        // The response from the agent is an async response, so we need to poll the status until it's completed.
        while (cancellationToken.IsCancellationRequested == false)
        {
            var statusResponse = await httpClient.GetAsync(invokeAsyncResult.StatusQueryGetUri, cancellationToken);
            statusResponse.EnsureSuccessStatusCode();
            var status = await statusResponse.Content.ReadFromJsonAsync<OrchestrationStatus>(_jsonSerializerOptions, cancellationToken)
                ?? throw new InvalidOperationException("The format of the status response from the agent is invalid.");
            if (status.RuntimeStatus == "Completed")
            {
                return status.Output ?? throw new InvalidOperationException("The format of the output from the agent is invalid.");
            }

            await Task.Delay(1000, cancellationToken);
        }

        // If the cancellation token is requested, terminate the agent response.
        await httpClient.PostAsync(invokeAsyncResult.TerminatePostUri, null, CancellationToken.None);
        throw new TaskCanceledException("The agent response is not ready.");
    }

    record InvokeAsyncResult(string Id, string StatusQueryGetUri, string TerminatePostUri);
    record class OrchestrationStatus(string RuntimeStatus, AgentResponseWithAdditionalInfoDto? Output);
}
