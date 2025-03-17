using DurableMultiAgentTemplate.Agent;
using DurableMultiAgentTemplate.Agent.AgentDecider;
using DurableMultiAgentTemplate.Agent.Orchestrator;
using DurableMultiAgentTemplate.Agent.Synthesizer;
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
        // Arrange: Setup mock context and input data
        Mock<TaskOrchestrationContext> contextMock = new();
        contextMock.Setup(x => x.CreateReplaySafeLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        var reqData = new AgentRequestDto([]);
        contextMock.Setup(x => x.GetInput<AgentRequestDto>())
            .Returns(reqData);

        // Capture the custom status set by the orchestrator
        AgentOrchestratorStatus? status = null;
        contextMock.Setup(x => x.SetCustomStatus(It.IsAny<AgentOrchestratorStatus>()))
            .Callback<object?>(param => status = param as AgentOrchestratorStatus);

        // Simulate cancellation of the activity
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();
        contextMock.Setup(x => x.CallActivityAsync<AgentDeciderResult>(AgentActivityName.AgentDeciderActivity, reqData, It.IsAny<TaskOptions>()))
            .Returns(Task.FromCanceled<AgentDeciderResult>(cancellationTokenSource.Token));

        // Act: Run the orchestrator and expect a TaskCanceledException
        var orchestrator = new AgentOrchestrator();
        await Assert.ThrowsAsync<TaskCanceledException>(async () => await orchestrator.RunOrchestrator(contextMock.Object));

        // Assert: Verify the custom status was set correctly
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
        // Arrange: Setup mock context and input data
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

        // Act: Run the orchestrator
        var orchestrator = new AgentOrchestrator();
        var orchestratorResult = await orchestrator.RunOrchestrator(contextMock.Object);

        // Assert: Verify the result is of type AgentResponseDto and has the expected content
        Assert.IsInstanceOfType<AgentResponseDto>(orchestratorResult);
        Assert.AreEqual("No agent call", orchestratorResult.Content);
    }

    [TestMethod]
    public async Task RunOrchestrator_ShouldReturnAgentResponseWithAdditionalInfoDto_WhenNoAgentCallAndAdditionalInfoIsTrue()
    {
        // Arrange: Setup mock context and input data
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

        // Act: Run the orchestrator
        var orchestrator = new AgentOrchestrator();
        var orchestratorResult = await orchestrator.RunOrchestrator(contextMock.Object);

        // Assert: Verify the result is of type AgentResponseWithAdditionalInfoDto and has the expected content
        Assert.IsInstanceOfType<AgentResponseWithAdditionalInfoDto>(orchestratorResult);
        Assert.AreEqual("No agent call", orchestratorResult.Content);
    }

    [TestMethod]
    public async Task RunOrchestrator_ShouldThrowArgumentNullException_WhenRequestDataIsNull()
    {
        // Arrange: Setup mock context with null input data
        Mock<TaskOrchestrationContext> contextMock = new();
        contextMock.Setup(x => x.CreateReplaySafeLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        contextMock.Setup(x => x.GetInput<AgentRequestDto>())
            .Returns((AgentRequestDto)null!);
        
        var orchestrator = new AgentOrchestrator();

        // Act & Assert: Run the orchestrator and expect an ArgumentNullException
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => orchestrator.RunOrchestrator(contextMock.Object));
    }

    [TestMethod]
    public async Task SetCustomStatus_WhenWorkerAgentActivityIsCalled()
    {
        // Arrange: Setup mock context and input data
        Mock<TaskOrchestrationContext> contextMock = new();
        contextMock.Setup(x => x.CreateReplaySafeLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        
        var reqData = new AgentRequestDto([]);
        contextMock.Setup(x => x.GetInput<AgentRequestDto>())
            .Returns(reqData);

        // Setup agent decider result with multiple agent calls
        List<AgentCall> agentCalls = [
            new AgentCall("TestAgent1", "Argument1"),
            new AgentCall("TestAgent2", "Argument2")
        ];
        var agentDeciderResult = new AgentDeciderResult(true, "Agent call", agentCalls);
        
        contextMock.Setup(x => x.CallActivityAsync<AgentDeciderResult>(
            AgentActivityName.AgentDeciderActivity, reqData, It.IsAny<TaskOptions>()))
            .ReturnsAsync(agentDeciderResult);
        
        // Capture the custom statuses set by the orchestrator
        List<AgentOrchestratorStatus> statuses = [];
        contextMock.Setup(x => x.SetCustomStatus(It.IsAny<AgentOrchestratorStatus>()))
            .Callback<object?>(param => 
            {
                if (param is AgentOrchestratorStatus status)
                    statuses.Add(status);
            });
        
        // Simulate cancellation of the worker agent activity
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();
        
        contextMock.Setup(x => x.CallActivityAsync<string>(It.IsAny<TaskName>(), It.IsAny<object>(), It.IsAny<TaskOptions>()))
            .Returns(Task.FromCanceled<string>(cancellationTokenSource.Token));

        // Act: Run the orchestrator and expect a TaskCanceledException
        var orchestrator = new AgentOrchestrator();
        await Assert.ThrowsAsync<TaskCanceledException>(async () => await orchestrator.RunOrchestrator(contextMock.Object));

        // Assert: Verify the custom status for WorkerAgentActivity was set correctly
        Assert.IsTrue(statuses.Count >= 2);
        Assert.AreEqual(AgentOrchestratorStep.WorkerAgentActivity, statuses[1].Step);
        CollectionAssert.AreEqual(agentCalls.ToList(), statuses[1].AgentCalls.ToList());
    }

    [TestMethod]
    public async Task RunOrchestrator_ShouldCallSynthesizerActivity_WhenAgentCallAndAdditionalInfoIsFalse()
    {
        // Arrange: Setup mock context and input data
        Mock<TaskOrchestrationContext> contextMock = new();
        contextMock.Setup(x => x.CreateReplaySafeLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        
        var reqData = new AgentRequestDto([], false);
        contextMock.Setup(x => x.GetInput<AgentRequestDto>())
            .Returns(reqData);
        
        // Setup agent decider result with multiple agent calls
        List<AgentCall> agentCalls = [
            new AgentCall("TestAgent1", "Argument1"),
            new AgentCall("TestAgent2", "Argument2"),
        ];
        
        var agentDeciderResult = new AgentDeciderResult(true, "Agent call", agentCalls);
        contextMock.Setup(x => x.CallActivityAsync<AgentDeciderResult>(
            AgentActivityName.AgentDeciderActivity, reqData, It.IsAny<TaskOptions>()))
            .ReturnsAsync(agentDeciderResult);
        
        // Setup agent call results
        contextMock.Setup(x => x.CallActivityAsync<string>("TestAgent1", "Argument1", It.IsAny<TaskOptions>()))
            .ReturnsAsync("Agent1Result");
        contextMock.Setup(x => x.CallActivityAsync<string>("TestAgent2", "Argument2", It.IsAny<TaskOptions>()))
            .ReturnsAsync("Agent2Result");

        // Track synthesizer request
        SynthesizerRequest? capturedSynthesizerRequest = null;
        contextMock.Setup(x => x.CallActivityAsync<AgentResponseDto>(
            AgentActivityName.SynthesizerActivity, It.IsAny<SynthesizerRequest>(), It.IsAny<TaskOptions>()))
            .Callback<TaskName, object?, TaskOptions?>((_, obj, _) => capturedSynthesizerRequest = obj as SynthesizerRequest)
            .ReturnsAsync(new AgentResponseDto("Synthesized result"));

        // Act: Run the orchestrator
        var orchestrator = new AgentOrchestrator();
        var result = await orchestrator.RunOrchestrator(contextMock.Object);

        // Assert: Verify the synthesizer request and result
        Assert.IsNotNull(capturedSynthesizerRequest);
        Assert.AreEqual(2, capturedSynthesizerRequest.AgentCallResult.Count);
        CollectionAssert.Contains(capturedSynthesizerRequest.AgentCallResult, "Agent1Result");
        CollectionAssert.Contains(capturedSynthesizerRequest.AgentCallResult, "Agent2Result");
        CollectionAssert.Contains(capturedSynthesizerRequest.CalledAgentNames, "TestAgent1");
        CollectionAssert.Contains(capturedSynthesizerRequest.CalledAgentNames, "TestAgent2");
        Assert.AreEqual("Synthesized result", result.Content);
    }

    [TestMethod]
    public async Task RunOrchestrator_ShouldCallSynthesizerWithAdditionalInfoActivity_WhenAgentCallAndAdditionalInfoIsTrue()
    {
        // Arrange: Setup mock context and input data
        Mock<TaskOrchestrationContext> contextMock = new();
        contextMock.Setup(x => x.CreateReplaySafeLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        
        var reqData = new AgentRequestDto([], true);
        contextMock.Setup(x => x.GetInput<AgentRequestDto>())
            .Returns(reqData);

        // Setup agent decider result with a single agent call
        List<AgentCall> agentCalls = [
            new AgentCall("TestAgent1", "Argument1")
        ];
        
        var agentDeciderResult = new AgentDeciderResult(true, "Agent call", agentCalls);
        contextMock.Setup(x => x.CallActivityAsync<AgentDeciderResult>(
            AgentActivityName.AgentDeciderActivity, reqData, It.IsAny<TaskOptions>()))
            .ReturnsAsync(agentDeciderResult);
        
        // Setup agent call result
        contextMock.Setup(x => x.CallActivityAsync<string>("TestAgent1", "Argument1", It.IsAny<TaskOptions>()))
            .ReturnsAsync("Agent1Result");

        // Track synthesizer request and return response with additional info
        SynthesizerRequest? capturedSynthesizerRequest = null;
        contextMock.Setup(x => x.CallActivityAsync<AgentResponseWithAdditionalInfoDto>(
            AgentActivityName.SynthesizerWithAdditionalInfoActivity, It.IsAny<SynthesizerRequest>(), It.IsAny<TaskOptions>()))
            .Callback<TaskName, object, TaskOptions>((_, obj, _) => capturedSynthesizerRequest = (SynthesizerRequest)obj)
            .ReturnsAsync(new AgentResponseWithAdditionalInfoDto("Synthesized result with additional info"));

        // Capture the custom statuses set by the orchestrator
        List<AgentOrchestratorStatus> statuses = [];
        contextMock.Setup(x => x.SetCustomStatus(It.IsAny<AgentOrchestratorStatus>()))
            .Callback<object?>(param => 
            {
                if (param is AgentOrchestratorStatus status)
                    statuses.Add(status);
            });

        // Act: Run the orchestrator
        var orchestrator = new AgentOrchestrator();
        var result = await orchestrator.RunOrchestrator(contextMock.Object);

        // Assert: Verify the synthesizer request and result
        Assert.IsNotNull(capturedSynthesizerRequest);
        Assert.AreEqual(1, capturedSynthesizerRequest.AgentCallResult.Count);
        CollectionAssert.Contains(capturedSynthesizerRequest.AgentCallResult, "Agent1Result");
        CollectionAssert.Contains(capturedSynthesizerRequest.CalledAgentNames, "TestAgent1");
        
        Assert.IsInstanceOfType<AgentResponseWithAdditionalInfoDto>(result);
        Assert.AreEqual("Synthesized result with additional info", result.Content);
        
        // Verify SynthesizerActivity status was set
        Assert.IsTrue(statuses.Count >= 3);
        Assert.AreEqual(AgentOrchestratorStep.SynthesizerActivity, statuses[2].Step);
    }

    [TestMethod]
    public async Task SetCustomStatus_WhenSynthesizerActivityIsCalled()
    {
        // Arrange: Setup mock context and input data
        Mock<TaskOrchestrationContext> contextMock = new();
        contextMock.Setup(x => x.CreateReplaySafeLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        
        var reqData = new AgentRequestDto([], false);
        contextMock.Setup(x => x.GetInput<AgentRequestDto>())
            .Returns(reqData);

        // Setup agent decider result with a single agent call
        List<AgentCall> agentCalls = [new AgentCall("TestAgent", "Argument")];
        var agentDeciderResult = new AgentDeciderResult(true, "Agent call", agentCalls);
        
        contextMock.Setup(x => x.CallActivityAsync<AgentDeciderResult>(
            AgentActivityName.AgentDeciderActivity, reqData, It.IsAny<TaskOptions>()))
            .ReturnsAsync(agentDeciderResult);
        
        // Setup agent call result
        contextMock.Setup(x => x.CallActivityAsync<string>("TestAgent", "Argument", It.IsAny<TaskOptions>()))
            .ReturnsAsync("AgentResult");

        // Capture the custom statuses set by the orchestrator
        List<AgentOrchestratorStatus> statuses = [];
        contextMock.Setup(x => x.SetCustomStatus(It.IsAny<AgentOrchestratorStatus>()))
            .Callback<object?>(param => 
            {
                if (param is AgentOrchestratorStatus status)
                    statuses.Add(status);
            });
        
        // Simulate cancellation of the synthesizer activity
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();
        
        contextMock.Setup(x => x.CallActivityAsync<AgentResponseDto>(
            AgentActivityName.SynthesizerActivity, It.IsAny<SynthesizerRequest>(), It.IsAny<TaskOptions>()))
            .Returns(Task.FromCanceled<AgentResponseDto>(cancellationTokenSource.Token));

        // Act: Run the orchestrator and expect a TaskCanceledException
        var orchestrator = new AgentOrchestrator();
        await Assert.ThrowsAsync<TaskCanceledException>(async () => await orchestrator.RunOrchestrator(contextMock.Object));
        
        // Assert: Verify the custom status for SynthesizerActivity was set correctly
        Assert.IsTrue(statuses.Count >= 3);
        Assert.AreEqual(AgentOrchestratorStep.SynthesizerActivity, statuses[2].Step);
        var synthesizerArgs = statuses[2].AgentCalls.Single().Arguments as SynthesizerRequest;
        Assert.IsNotNull(synthesizerArgs);
        Assert.AreEqual(reqData, synthesizerArgs.AgentRequest);
        CollectionAssert.AreEqual(new[] { "TestAgent" }, synthesizerArgs.CalledAgentNames);
        CollectionAssert.AreEqual(new[] { "AgentResult" }, synthesizerArgs.AgentCallResult);
    }
}
