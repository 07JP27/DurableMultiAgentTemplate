using DurableMultiAgentTemplate.Model;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using DurableMultiAgentTemplate.Shared.Model;
using DurableMultiAgentTemplate.Agent.AgentDecider;
using DurableMultiAgentTemplate.Agent.Synthesizer;
using System.Text.Json.Nodes;
using System.Text.Json;
using DurableMultiAgentTemplate.Agent.Workers;

namespace DurableMultiAgentTemplate.Agent.Orchestrator;

public class AgentOrchestrator()
{
    private static TaskOptions DefaultTaskOptions { get; } = new(
        new TaskRetryOptions(new RetryPolicy(
            3, 
            TimeSpan.FromSeconds(1),
            1,
            TimeSpan.FromSeconds(10))));

    [Function(nameof(AgentOrchestrator))]
    public async Task<AgentResponseDto> RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var logger = context.CreateReplaySafeLogger("AgentOrchestrator");
        var reqData = context.GetInput<AgentRequestDto>();

        ArgumentNullException.ThrowIfNull(reqData);

        context.SetCustomStatus(new AgentOrchestratorStatus(AgentOrchestratorStep.AgentDeciderActivity,
            [new AgentCall(AgentActivityNames.AgentDeciderActivity, JsonSerializer.SerializeToElement(reqData))]));
        // AgentDecider呼び出し（呼び出すAgentの決定）
        var agentDeciderResult = await context.CallActivityAsync<AgentDeciderResult>(AgentActivityNames.AgentDeciderActivity, reqData, DefaultTaskOptions);

        // AgentDeciderでエージェントを呼び出さない場合には、そのまま返す
        if (!agentDeciderResult.IsAgentCall)
        {
            logger.LogInformation("No agent call happened");
            if (reqData.RequireAdditionalInfo)
            {
                return new AgentResponseWithAdditionalInfoDto(new(agentDeciderResult.Content));
            }
            else
            {
                return new AgentResponseDto(new(agentDeciderResult.Content));
            }
        }

        // Agent呼び出し
        logger.LogInformation("Agent call happened");
        context.SetCustomStatus(
            new AgentOrchestratorStatus(AgentOrchestratorStep.WorkerAgentActivity, agentDeciderResult.AgentCalls));
        var parallelAgentCall = new List<Task<WorkerAgentResult>>();
        foreach (var agentCall in agentDeciderResult.AgentCalls)
        {
            parallelAgentCall.Add(context.CallActivityAsync<WorkerAgentResult>(agentCall.AgentName, agentCall.Arguments, DefaultTaskOptions));
        }

        await Task.WhenAll(parallelAgentCall);

        // Synthesizer呼び出し（回答集約）
        SynthesizerRequest synthesizerRequest = new(
            AgentCallResult: [.. parallelAgentCall.Select(x => x.Result)],
            AgentRequest: reqData,
            CalledAgentNames: [.. agentDeciderResult.AgentCalls.Select(x => x.AgentName)]
        );
        
        context.SetCustomStatus(new AgentOrchestratorStatus(AgentOrchestratorStep.SynthesizerActivity,
            [new AgentCall(AgentActivityNames.SynthesizerActivity, JsonSerializer.SerializeToElement(synthesizerRequest))]));
        if (reqData.RequireAdditionalInfo)
        {
            return await context.CallActivityAsync<AgentResponseWithAdditionalInfoDto>(AgentActivityNames.SynthesizerWithAdditionalInfoActivity, synthesizerRequest, DefaultTaskOptions);
        }
        else
        {
            return await context.CallActivityAsync<AgentResponseDto>(AgentActivityNames.SynthesizerActivity, synthesizerRequest, DefaultTaskOptions);
        }
    }
}
