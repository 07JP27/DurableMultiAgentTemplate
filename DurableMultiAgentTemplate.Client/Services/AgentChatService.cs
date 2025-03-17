using System.Text.Json;
using DurableMultiAgentTemplate.Shared.Model;

namespace DurableMultiAgentTemplate.Client.Services;

/// <summary>
/// Service to handle agent chat operations.
/// </summary>
public class AgentChatService(HttpClient httpClient, ILogger<AgentChatService> logger)
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    /// <summary>
    /// Gets the agent response asynchronously.
    /// </summary>
    /// <param name="agentRequestDto">The agent request DTO.</param>
    /// <param name="progress">The progress reporter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The agent response with additional info DTO.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the response format is invalid.</exception>
    /// <exception cref="TaskCanceledException">Thrown when the agent response is not ready.</exception>
    public async Task<AgentResponseWithAdditionalInfoDto> GetAgentResponseAsync(AgentRequestDto agentRequestDto,
        IProgress<AgentOrchestratorStatus>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("api/invoke/async", agentRequestDto, cancellationToken);
        response.EnsureSuccessStatusCode();

        var agentOrchestratorStatus = AgentOrchestratorStatus.NotStarted;
        progress?.Report(agentOrchestratorStatus);
        var invokeAsyncResult = await response.Content.ReadFromJsonAsync<InvokeAsyncResult>(_jsonSerializerOptions, cancellationToken) ?? 
            throw new InvalidOperationException("The format of the response from the agent is invalid.");

        // The response from the agent is an async response, so we need to poll the status until it's completed.
        while (cancellationToken.IsCancellationRequested == false)
        {
            var statusResponse = await httpClient.GetAsync(invokeAsyncResult.StatusQueryGetUri, cancellationToken);
            statusResponse.EnsureSuccessStatusCode();
            var status = await statusResponse.Content.ReadFromJsonAsync<OrchestrationStatus>(_jsonSerializerOptions, cancellationToken)
                ?? throw new InvalidOperationException("The format of the status response from the agent is invalid.");
            logger.LogInformation("Orchstrator status: {status}", status);
            if (status.RuntimeStatus == "Completed")
            {
                return status.Output ?? throw new InvalidOperationException("The format of the output from the agent is invalid.");
            }

            if (status.CustomStatus != null && agentOrchestratorStatus.Step != status.CustomStatus.Step)
            {
                agentOrchestratorStatus = status.CustomStatus;
                progress?.Report(agentOrchestratorStatus);
            }

            await Task.Delay(1000, cancellationToken);
        }

        // If the cancellation token is requested, terminate the agent response.
        await httpClient.PostAsync(invokeAsyncResult.TerminatePostUri, null, CancellationToken.None);
        throw new TaskCanceledException("The agent response is not ready.");
    }

    private record InvokeAsyncResult(string Id, string StatusQueryGetUri, string TerminatePostUri);
    private record class OrchestrationStatus(
        string RuntimeStatus,
        AgentResponseWithAdditionalInfoDto? Output,
        AgentOrchestratorStatus? CustomStatus);
}
