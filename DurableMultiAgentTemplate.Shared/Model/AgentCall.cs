using System.Text.Json;
using System.Text.Json.Nodes;

namespace DurableMultiAgentTemplate.Shared.Model;


/// <summary>
/// Represents a call to an agent with a specified name and arguments.
/// </summary>
/// <param name="AgentName">The name of the agent being called.</param>
/// <param name="Arguments">The arguments to be passed to the agent.</param>
public record AgentCall(string AgentName, JsonElement Arguments);
