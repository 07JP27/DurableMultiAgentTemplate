namespace DurableMultiAgentTemplate.Agent;

internal static class AgentActivityName
{
    // Orchestrator Agent functions
    public const string AgentDeciderActivity = nameof(AgentDeciderActivity);
    public const string SynthesizerActivity = nameof(SynthesizerActivity);
    public const string SynthesizerWithAdditionalInfoActivity = nameof(SynthesizerWithAdditionalInfoActivity);

    // Each Agent
    public const string GetDestinationSuggestAgent = nameof(GetDestinationSuggestAgent);
    public const string GetClimateAgent = nameof(GetClimateAgent);
    public const string GetSightseeingSpotAgent = nameof(GetSightseeingSpotAgent);
    public const string GetHotelAgent = nameof(GetHotelAgent);
    public const string SubmitReservationAgent = nameof(SubmitReservationAgent);
}