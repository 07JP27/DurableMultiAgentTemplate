[日本語版はこちら](README_ja.md)

![MIT](https://img.shields.io/badge/license-MIT-blue)
![GitHub watchers](https://img.shields.io/github/watchers/07JP27/DurableMultiAgentTemplate)
![GitHub forks](https://img.shields.io/github/forks/07JP27/DurableMultiAgentTemplate)
![GitHub Repo stars](https://img.shields.io/github/stars/07JP27/DurableMultiAgentTemplate)

# DurableFunctions template - Orchestrator-workers Multi-Agent 

This repository is a template project for implementing the Orchestrator-Workers pattern introduced in Anthropic's blog "[Building effective agents](https://www.anthropic.com/research/building-effective-agents)" using Azure Durable Functions.

![](https://www-cdn.anthropic.com/images/4zrzovbb/website/8985fc683fae4780fb34eab1365ab78c7e51bc8e-2401x1000.png)
ref: [Anthropic-Building effective agents](https://www.anthropic.com/research/building-effective-agents)

## Sequence Diagram
Case of synchronous endpoint:

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
    DurableOrchestrator ->> WorkerAgentActivity: Invoke(Multiple & Parallel)
    WorkerAgentActivity ->> DurableOrchestrator: Result
    DurableOrchestrator ->> SynthesizerActivity: Synthesize result & generate answer
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

Please modify the implementation of each agent, which currently uses fixed values, to include RAG, actions, and other processes, thereby creating agents that meet the requirements.
Each agent can utilize OpenAI Client, Cosmos DB Client and application configuration values provided by the DI container.

To demonstrate the retry functionality of Durable Functions, each Agent includes code that emulates failures in external service calls, such as those to an LLM.
Agent Activities fail randomly with a 30% probability during execution.
When implementing an actual Agent based on the sample Agent, please remove the following parts from the code.
```cs
if(Random.Shared.Next(0, 10) < 3)
{
    logger.LogInformation("Failed to get climate information");
    throw new InvalidOperationException("Failed to get climate information");
}
```

## Local Setup
0. Before running, create the following resources:
   - Azure OpenAI Service with a deployed chat completions model (embedding model deployment is optional)
   - Azure Cosmos DB (optional, if used)
      - Instead of Azure Cosmos DB, you can also use the Azure Cosmos DB Emulator. For more details, please refer to the documentation below.<br/>
      [Develop locally using the Azure Cosmos DB emulator](https://learn.microsoft.com/en-us/azure/cosmos-db/how-to-develop-emulator?tabs=windows%2Ccsharp&pivots=api-nosql)

1. Update the `local.settings.json` of the `DurableMultiAgentTemplate` project with your resource information below:   
   - Azure OpenAI endpoint and deployment name  
   - Azure Cosmos DB endpoint (optional if not used)  
     
   You can choose between API key or Entra ID authentication for each service:  
   - If using an API key: Include the API key in the `local.settings.json` file.  
   - If using Entra ID authentication: Authenticate using the `az login` command with Azure CLI. Leave the API key blank in the `local.settings.json` file. Note that **the authenticated user must have RBAC permissions for each service.**

2. Start a local storage account using Azurite

Execute the following command in a separate window from the main one:
```shell
azurite
```
Alternatively, run the following command to execute it in the background:
```shell
nohup azurite > azurite.log 2>&1 &
```

If azurite is not installed, install it using the following command.
```shell
npm install -g azurite
```

For more details, please refer to the following page:
https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=visual-studio%2Cblob-storage


3. Run the project.

## How to run in your local environment
You can use either the .NET Blazor client or the Python Streamlit client for testing the project.

## .NET client app to test

You can test the operation using a simple chat app in .NET as follows:

https://github.com/user-attachments/assets/10425f9a-cd55-4f02-8cd1-6a1935df4db0

### Visual Studio 2022

1. Open the solution file `DurableMultiAgentTemplate.sln` in Visual Studio 2022.
2. Select `Multi agent test` as the startup project.
   - This will run both the Durable Functions and .NET client projects simultaneously.
3. Press `F5` to run the projects.
   - If you encounter an error, please check the `local.settings.json` file in the `DurableMultiAgentTemplate` project.

### Visual Studio Code

1. From the activity bar in Visual Studio Code, select Run > Start Debugging.
2. From the dropdown list, select `C#: Attach to DurableMultiAgentTemplate worker process` and press the run button to the left of the dropdown list.
   - This will start the Functions host with the task `host start (functions)`.
   - In the Functions host terminal, a log like `"{ "name":"dotnet-worker-startup", "workerProcessId" : 12345 }"` will be output. The workerProcessId is the ID of the process to which the debugger will attach.
   - When the `prelaunchTask` dialog appears, select `Debug Anyway`.
   - A list of processes to attach the debugger will be displayed. Select the process that was output in the Functions host log earlier.
   - The debugger will attach and debugging will start.
3. From the dropdown list, select `C#: Debug DurableMultiAgentTemplate.Client` and press the run button to the left of the dropdown list.

After running the project, you can access the client at `http://localhost:{your port number}`.

## Python client
You can test the Orchestrator-Workers pattern using [Client/client.py]. 
It can be executed with the following command:    
```bash  
dotnet run --project .\DurableMultiAgentTemplate\DurableMultiAgentTemplate.csproj  
```  

The client is created with Streamlit, so it can be run using the following command:    
```bash  
streamlit run client.py  
```

![](Assets/demo.gif)
[The full-resolution video is here.](https://youtu.be/SACD4IyKQAI)

## API specifications hosted in the template
### Endpoints
There are two types of endpoints: synchronous and asynchronous. If the agent's processing takes a long time, it is recommended to use the asynchronous endpoint.
For more information about the asynchronous pattern in Durable Functions, [see here](https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-overview?tabs=in-process%2Cnodejs-v3%2Cv1-model&pivots=csharp#async-http).

- synchronous`http://{your base url}/api/invoke/sync`
- asynchronous`http://{your base url}/api/invoke/async`

### API　request
The request body will be below:
```json
{
	"messages": [
		{
			"role": "user",
			"content": "I'm finding a travel destination."
		},
		{
			"role": "assistant",
			"content": "Please tell me your desired conditions for the travel destination. For example, a place with a beach, a place with many historical tourist spots, a place with abundant nature, etc. I will suggest a travel destination according to your preferences."
		},
		{
			"role": "user",
			"content": "I like a place with many historical tourist spots."
		}
	],
	"requireAdditionalInfo": true
}
```

### Response
The response will be below:
```json
{
	"additionalInfo": [
		{
			"$type": "mardown",
			"markdownText": "### Recommended Travel Destinations with Historical Landmarks\n#### Domestic\n1. **Okinawa Main Island**\n  - Rich in tourist attractions such as crystal-clear beaches, Shurijo Castle, and the Churaumi Aquarium. \n- Warm weather even in winter, offering a relaxed atmosphere.\n2. **Ishigaki Island and Miyako Island**\n - Expansive beautiful natural landscapes typical of the tropics, with popular activities like diving and snorkeling.\n- Enjoy unique local cuisine of the islands.\n3. **Kagoshima and Amami Oshima**\n - Experience Amami's black sugar shochu, island songs, and distinctive natural environments.\n- Enjoy the subtropical atmosphere."
		}
	],
	"content": "Here are some recommended travel destinations with many historical landmarks:\n1. **Okinawa Main Island** - Features spots that blend history and tourism, such as Shurijo Castle and the Churaumi Aquarium.\n2. **Kagoshima and Amami Oshima** - Enjoy island songs and the subtropical atmosphere.\n3. **Ishigaki Island and Miyako Island** - Known for unique local cuisine and natural landscapes.\nFor more details, please refer to the additional information.",
	"calledAgentNames": [
		"GetDestinationSuggestAgent"
	]
}
```

![](Assets/demo.gif)
[The full-resolution video is here.](https://youtu.be/SACD4IyKQAI)

## Request for additional information
In general, responses from agents, especially when they include retrieval-augmented generation (RAG), can become lengthy and negatively affect the chat experience. To improve this, separating static additional information from the flow of the chat can enhance the user experience.

In this template, you can request additional information by setting `requireAdditionalInfo` to `true` when making a request. The additional information will be returned separately from the chat response and stored in the `additionalInfo` field.

By toggling the `REQUIRE_ADDITIONAL_INFO` flag in the client code for testing, you can experience this functionality in action. 
![](Assets/demo_additional.gif)

This feature also reduces the token count of the `messages` array in the chat history, making the LLM's performance lighter. 
However, in some cases, the lack of context in the `messages` array might result in unnatural agent responses. 
In such cases, you may consider merging the additional information back into the `messages` array and requesting the agent's response accordingly.

## Azure Environment Setup  
TBW
https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-isolated-create-first-csharp?pivots=code-editor-vscode#sign-in-to-azure
