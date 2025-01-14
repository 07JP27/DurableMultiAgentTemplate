using DurableMultiAgentTemplate.Model;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;

namespace DurableMultiAgentTemplate.Agent.Orchestrator;

public class AgentOrchestrator()
{
    [Function(nameof(AgentOrchestrator))]
    public async Task<AgentResponseDto> RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        ILogger logger = context.CreateReplaySafeLogger("AgentOrchestrator");
        var reqData = context.GetInput<AgentRequestDto>();

        ArgumentNullException.ThrowIfNull(reqData);

        // AgentDecider呼び出し（呼び出すAgentの決定）
        var AgentDeciderResult = await context.CallActivityAsync<AgentDeciderResult>(AgentActivityName.AgentDeciderActivity, reqData);

        if (!AgentDeciderResult.IsAgentCall)
        {
            logger.LogInformation("No agent call happened");
            return new AgentResponseDto
            {
                Content = AgentDeciderResult.Content
            };
        }

        // Agent呼び出し
        logger.LogInformation("Agent call happened");
        var parallelAgentCall = new List<Task<string>>();
        foreach (var agentCall in AgentDeciderResult.AgentCalls)
        {
            var args = agentCall.Arguments;
            parallelAgentCall.Add(context.CallActivityAsync<string>(agentCall.AgentName, args));
        }

        await Task.WhenAll(parallelAgentCall);

        // Synthesizer呼び出し（回答集約）
        SynthesizerRequest synthesizerRequest = new()
        {
            AgentCallResult = parallelAgentCall.Select(x => x.Result).ToList(),
            AgentReques = reqData,
            CalledAgentNames = AgentDeciderResult.AgentCalls.Select(x => x.AgentName).ToList()
        };

        if (reqData.RequireAdditionalInfo)
        {
            var res= await context.CallActivityAsync<AgentResponseWithAdditionalInfoDto>(AgentActivityName.SynthesizerWithAdditionalInfoActivity, synthesizerRequest);
            return res;
        }
        else
        {
            var res = await context.CallActivityAsync<AgentResponseDto>(AgentActivityName.SynthesizerActivity, synthesizerRequest);
            return res;
        }
    }
}
