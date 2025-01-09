[日本語版はこちら](README_ja.md)

# DurableFunctions template - Orchestrator-workers Multi-Agent 

This repository is a template project for implementing the Orchestrator-Workers pattern introduced in Anthropic's blog "[Building effective agents](https://www.anthropic.com/research/building-effective-agents)" using Azure Durable Functions.


![](https://www-cdn.anthropic.com/images/4zrzovbb/website/8985fc683fae4780fb34eab1365ab78c7e51bc8e-2401x1000.png)
ref: [Anthropic-Building effective agents](https://www.anthropic.com/research/building-effective-agents)

## Sequence Diagram
Case of synchronous endpoint.

```mermaid
sequenceDiagram
    participant Client
    participant Sarter
    participant DurableOrchestrator
    participant AgentDeciderActivity
    participant WorkerAgentActivity
    participant SynthesizerActivity

    Client->>Sarter: Web API request
    Sarter->>DurableOrchestrator: Start 
    DurableOrchestrator ->> AgentDeciderActivity: Decide agent to call
    AgentDeciderActivity ->> DurableOrchestrator: Agent that should call
    DurableOrchestrator -->> Sarter: If no agent to call, return plain text
    DurableOrchestrator ->> WorkerAgentActivity: Invoke(Multiple and Parallel)
    WorkerAgentActivity ->> DurableOrchestrator: Result
    DurableOrchestrator ->> SynthesizerActivity: Synthesize result amn generate answer
    SynthesizerActivity ->> DurableOrchestrator: Synthesized answer
    DurableOrchestrator ->> Sarter: Answer
    Sarter ->> Client: Web API respons
```

## Agent sample
This Multi-Agent system is based on a travel concierge scenario.
Each Agent is set to return fixed values as responses for sample purposes.
The sample Agents defined in the template are as follows:
- GetDestinationSuggestAgent：Get the destination suggestion
- GetClimateAgent：Get the climate of the destination
- GetSightseeingSpotAgent：Get the sightseeing spot of the destination
- GetHotelAgent：Get the hotel information of the destination
- SubmitReservationAgent：Submit the reservation of the hotel

## Endpoints
There are two types of endpoints: synchronous and asynchronous. If the agent's processing takes a long time, it is recommended to use the asynchronous endpoint.
For more information about the asynchronous pattern in Durable Functions, [see here](https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-overview?tabs=in-process%2Cnodejs-v3%2Cv1-model&pivots=csharp#async-http).

## Client to test
You can use [client.py](client.py) to test the Orchestrator-Workers pattern.
This client made with Streamlit. So you can run it with the following command:
```bash
streamlit run client.py
```