using System.ComponentModel;

namespace DurableMultiAgentTemplate.Agent.Workers.HumanInTheLoopAgent;

public record HumanInTheLoopRequest(
    [property: Description("ユーザーが行いたい処理の詳細")]
    string UserRequest);
