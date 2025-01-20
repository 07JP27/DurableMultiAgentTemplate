using DurableMultiAgentTemplate.Model;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;

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
        ILogger logger = context.CreateReplaySafeLogger("AgentOrchestrator");
        var reqData = context.GetInput<AgentRequestDto>();

        ArgumentNullException.ThrowIfNull(reqData);

        // AgentDecider呼び出し（呼び出すAgentの決定）
        var AgentDeciderResult = await context.CallActivityAsync<AgentDeciderResult>(AgentActivityName.AgentDeciderActivity, reqData, DefaultTaskOptions);

        // AgentDeciderでエージェントを呼び出さない場合には、そのまま返す
        if (!AgentDeciderResult.IsAgentCall)
        {
            logger.LogInformation("No agent call happened");
            if (reqData.RequireAdditionalInfo)
            {
                return new AgentResponseWithAdditionalInfoDto{Content = AgentDeciderResult.Content};
            }
            else
            {
                return new AgentResponseDto{Content = AgentDeciderResult.Content};
            }
        }

        // Agent呼び出し
        logger.LogInformation("Agent call happened");
        var parallelAgentCall = new List<Task<string>>();
        foreach (var agentCall in AgentDeciderResult.AgentCalls)
        {
            var args = agentCall.Arguments;
            parallelAgentCall.Add(context.CallActivityAsync<string>(agentCall.AgentName, args, DefaultTaskOptions));
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
            var res= await context.CallActivityAsync<AgentResponseWithAdditionalInfoDto>(AgentActivityName.SynthesizerWithAdditionalInfoActivity, synthesizerRequest, DefaultTaskOptions);
            return res;
        }
        else
        {
            var res = await context.CallActivityAsync<AgentResponseDto>(AgentActivityName.SynthesizerActivity, synthesizerRequest, DefaultTaskOptions);
            return res;
        }
    }
}
