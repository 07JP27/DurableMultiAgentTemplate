namespace DurableMultiAgentTemplate.Model
{
    public class SubmitReservationRequest
    {
        public string Destination { get; set; } = string.Empty;
        public string CheckIn { get; set; } = string.Empty;
        public string CheckOut { get; set; } = string.Empty;
        public int GuestsCount { get; set; }
    }
}