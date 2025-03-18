using DurableMultiAgentTemplate.Json;
using DurableMultiAgentTemplate.Model;
using OpenAI.Chat;

namespace DurableMultiAgentTemplate.Agent.Workers;

//https://learn.microsoft.com/ja-jp/azure/ai-services/openai/how-to/dotnet-migration?tabs=stable
public record AgentDefinition(string AgentActivityName, ChatTool ChatTool, bool RequiresUserConfirmation);

public class AgentDefinitions(JsonUtilities jsonUtilities)
{
    private readonly AgentDefinition _getDestinationSuggestAgent = new(
        AgentActivityNames.GetDestinationSuggestAgent,
        ChatTool.CreateFunctionTool(
            functionName: AgentActivityNames.GetDestinationSuggestAgent,
            functionDescription: "希望の行き先に求める条件を自然言語で与えると、おすすめの旅行先を提案します。",
            functionParameters: jsonUtilities.GenerateSchemaAsBinaryData(SourceGenerationContext.Default.GetDestinationSuggestRequest)),
        false);

    private readonly AgentDefinition _getClimateAgent = new(
        AgentActivityNames.GetClimateAgent,
        ChatTool.CreateFunctionTool(
            functionName: AgentActivityNames.GetClimateAgent,
            functionDescription: "指定された場所の気候を取得します。",
            functionParameters: jsonUtilities.GenerateSchemaAsBinaryData(SourceGenerationContext.Default.GetClimateRequest)),
        false);

    private readonly AgentDefinition _getSightseeingSpotAgent = new(
        AgentActivityNames.GetSightseeingSpotAgent,
        ChatTool.CreateFunctionTool(
            functionName: AgentActivityNames.GetSightseeingSpotAgent,
            functionDescription: "指定された場所の観光名所を取得します。",
            functionParameters: jsonUtilities.GenerateSchemaAsBinaryData(SourceGenerationContext.Default.GetSightseeingSpotRequest)),
        false);

    private readonly AgentDefinition _getHotelAgent = new(
        AgentActivityNames.GetHotelAgent,
        ChatTool.CreateFunctionTool(
            functionName: AgentActivityNames.GetHotelAgent,
            functionDescription: "指定された場所のホテルを取得します。",
            functionParameters: jsonUtilities.GenerateSchemaAsBinaryData(SourceGenerationContext.Default.GetHotelRequest)),
        false);

    private readonly AgentDefinition _submitReservationAgent = new(
        AgentActivityNames.SubmitReservationAgent,
        ChatTool.CreateFunctionTool(
            functionName: AgentActivityNames.SubmitReservationAgent,
            functionDescription: "宿泊先の予約を行います。",
            functionParameters: jsonUtilities.GenerateSchemaAsBinaryData(SourceGenerationContext.Default.HotelReservationRequest)),
        false);

    private readonly AgentDefinition _commitReservationAgent = new(
        AgentActivityNames.CommitReservationAgent,
        ChatTool.CreateFunctionTool(
            functionName: AgentActivityNames.CommitReservationAgent,
            functionDescription: "宿泊先の予約の確定を行います。",
            functionParameters: jsonUtilities.GenerateSchemaAsBinaryData(SourceGenerationContext.Default.HotelReservationRequest)),
        true);

    public AgentDefinition[] AllAgents =>
    [
        _getDestinationSuggestAgent,
        _getClimateAgent,
        _getHotelAgent,
        _getSightseeingSpotAgent,
        _submitReservationAgent,
        _commitReservationAgent
    ];

    public AgentDefinition[] GetAgentDefinitions(bool requiresUserConfirmation) =>
        [.. AllAgents.Where(agent => agent.RequiresUserConfirmation == requiresUserConfirmation)];
}
