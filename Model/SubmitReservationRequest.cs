using System.ComponentModel;

namespace DurableMultiAgentTemplate.Model;

public class SubmitReservationRequest
{
    [Description("行き先のホテルの名前。")]
    public required string Destination { get; set; } = string.Empty;
    [Description("チェックイン日。YYYY/MM/DD形式。")]
    public required string CheckIn { get; set; } = string.Empty;
    [Description("チェックアウト日。YYYY/MM/DD形式。")]
    public required string CheckOut { get; set; } = string.Empty;
    [Description("宿泊人数。")]
    public required int GuestsCount { get; set; }
}
