using System.ComponentModel;

namespace DurableMultiAgentTemplate.Agent.GetHotelAgent;

/// <summary>
/// Record representing a request to retrieve hotel information.
/// Used as input for the GetHotelActivity.
/// </summary>
public record GetHotelRequest(
    [property: Description("場所の名前。例: ボストン, 東京、フランス")]
    string Location);
