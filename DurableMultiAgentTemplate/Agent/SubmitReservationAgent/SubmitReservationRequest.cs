using System.ComponentModel;

namespace DurableMultiAgentTemplate.Agent.SubmitReservationAgent;

/// <summary>
/// Record representing a request to submit a hotel reservation.
/// Used as input for the SubmitReservationActivity.
/// </summary>
public record SubmitReservationRequest(
    [property: Description("行き先のホテルの名前。")]
    string Destination,
    [property: Description("チェックイン日。YYYY/MM/DD形式。")]
    string CheckIn,
    [property: Description("チェックアウト日。YYYY/MM/DD形式。")]
    string CheckOut,
    [property: Description("宿泊人数。")]
    int GuestsCount);
