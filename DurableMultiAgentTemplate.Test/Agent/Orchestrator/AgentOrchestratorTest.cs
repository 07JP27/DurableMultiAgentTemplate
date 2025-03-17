using DurableMultiAgentTemplate.Agent;
using DurableMultiAgentTemplate.Agent.AgentDecider;
using DurableMultiAgentTemplate.Agent.Orchestrator;
using DurableMultiAgentTemplate.Shared.Model;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using Moq;

namespace DurableMultiAgentTemplate.Test.Agent.Orchestrator;

[TestClass]
public class AgentOrchestratorTest
{
    [TestMethod]
    public async Task SetCustomStatus_WhenAgentDeciderActivityIsCalled()
    {
        Mock<TaskOrchestrationContext> contextMock = new();
        contextMock.Setup(x => x.CreateReplaySafeLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        var reqData = new AgentRequestDto([]);
        contextMock.Setup(x => x.GetInput<AgentRequestDto>())
            .Returns(reqData);

        AgentOrchestratorStatus? status = null;
        contextMock.Setup(x => x.SetCustomStatus(It.IsAny<AgentOrchestratorStatus>()))
            .Callback<object?>(param => status = param as AgentOrchestratorStatus);

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();
        contextMock.Setup(x => x.CallActivityAsync<AgentDeciderResult>(AgentActivityName.AgentDeciderActivity, reqData, It.IsAny<TaskOptions>()))
            .Returns(Task.FromCanceled<AgentDeciderResult>(cancellationTokenSource.Token));

        var orchestrator = new AgentOrchestrator();
        await Assert.ThrowsAsync<TaskCanceledException>(async () => await orchestrator.RunOrchestrator(contextMock.Object));

        Assert.IsNotNull(status);
        Assert.AreEqual(AgentOrchestratorStep.AgentDeciderActivity, status.Step);
        CollectionAssert.AreEqual(
            new List<AgentCall>()
            {
                new (AgentActivityName.AgentDeciderActivity, reqData)
            },
            status.AgentCalls.ToList());
    }

    [TestMethod]
    public async Task RunOrchestrator_ShouldReturnAgentResponseDto_WhenNoAgentCallAndAdditionalInfoIsFalse()
    {
        Mock<TaskOrchestrationContext> contextMock = new();
        contextMock.Setup(x => x.CreateReplaySafeLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        var reqData = new AgentRequestDto([], false);
        contextMock.Setup(x => x.GetInput<AgentRequestDto>())
            .Returns(reqData);
        contextMock.Setup(x => x.CallActivityAsync<AgentDeciderResult>(
            AgentActivityName.AgentDeciderActivity, reqData,
            It.IsAny<TaskOptions>()))
            .ReturnsAsync(new AgentDeciderResult(IsAgentCall: false, Content: "No agent call", []));

        var orchestrator = new AgentOrchestrator();
        var orchestratorResult = await orchestrator.RunOrchestrator(contextMock.Object);
        Assert.IsInstanceOfType<AgentResponseDto>(orchestratorResult);
        Assert.AreEqual("No agent call", orchestratorResult.Content);
    }

    [TestMethod]
    public async Task RunOrchestrator_ShouldReturnAgentResponseWithAdditionalInfoDto_WhenNoAgentCallAndAdditionalInfoIsTrue()
    {
        Mock<TaskOrchestrationContext> contextMock = new();
        contextMock.Setup(x => x.CreateReplaySafeLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        var reqData = new AgentRequestDto([], true);
        contextMock.Setup(x => x.GetInput<AgentRequestDto>())
            .Returns(reqData);
        contextMock.Setup(x => x.CallActivityAsync<AgentDeciderResult>(
            AgentActivityName.AgentDeciderActivity, reqData,
            It.IsAny<TaskOptions>()))
            .ReturnsAsync(new AgentDeciderResult(IsAgentCall: false, Content: "No agent call", []));

        var orchestrator = new AgentOrchestrator();
        var orchestratorResult = await orchestrator.RunOrchestrator(contextMock.Object);
        Assert.IsInstanceOfType<AgentResponseWithAdditionalInfoDto>(orchestratorResult);
        Assert.AreEqual("No agent call", orchestratorResult.Content);
    }
}
