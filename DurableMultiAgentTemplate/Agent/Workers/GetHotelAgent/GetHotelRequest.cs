using System.ComponentModel;

namespace DurableMultiAgentTemplate.Agent.Workers.GetHotelAgent;

public record GetHotelRequest(
    [property: Description("場所の名前。例: ボストン, 東京、フランス")]
    string Location);
