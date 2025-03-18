using DurableMultiAgentTemplate.Json;
using DurableMultiAgentTemplate.Model;
using OpenAI.Chat;

namespace DurableMultiAgentTemplate.Agent.Workers;

//https://learn.microsoft.com/ja-jp/azure/ai-services/openai/how-to/dotnet-migration?tabs=stable
public record AgentDefinition(string AgentActivityName, ChatTool ChatTool, bool RequiresUserConfirmation);

public class AgentDefinitions
{
    public static readonly AgentDefinition GetDestinationSuggestAgent = new(
        AgentActivityNames.GetDestinationSuggestAgent,
        ChatTool.CreateFunctionTool(
            functionName: AgentActivityNames.GetDestinationSuggestAgent,
            functionDescription: "希望の行き先に求める条件を自然言語で与えると、おすすめの旅行先を提案します。",
            functionParameters: JsonSchemaGenerator.GenerateSchemaAsBinaryData(SourceGenerationContext.Default.GetDestinationSuggestRequest)),
        false);

    public static readonly AgentDefinition GetClimateAgent = new(
        AgentActivityNames.GetClimateAgent,
        ChatTool.CreateFunctionTool(
            functionName: AgentActivityNames.GetClimateAgent,
            functionDescription: "指定された場所の気候を取得します。",
            functionParameters: JsonSchemaGenerator.GenerateSchemaAsBinaryData(SourceGenerationContext.Default.GetClimateRequest)),
        false);

    public static readonly AgentDefinition GetSightseeingSpotAgent = new(
        AgentActivityNames.GetSightseeingSpotAgent,
        ChatTool.CreateFunctionTool(
            functionName: AgentActivityNames.GetSightseeingSpotAgent,
            functionDescription: "指定された場所の観光名所を取得します。",
            functionParameters: JsonSchemaGenerator.GenerateSchemaAsBinaryData(SourceGenerationContext.Default.GetSightseeingSpotRequest)),
        false);

    public static readonly AgentDefinition GetHotelAgent = new(
        AgentActivityNames.GetHotelAgent,
        ChatTool.CreateFunctionTool(
            functionName: AgentActivityNames.GetHotelAgent,
            functionDescription: "指定された場所のホテルを取得します。",
            functionParameters: JsonSchemaGenerator.GenerateSchemaAsBinaryData(SourceGenerationContext.Default.GetHotelRequest)),
        false);

    public static readonly AgentDefinition SubmitReservationAgent = new(
        AgentActivityNames.SubmitReservationAgent,
        ChatTool.CreateFunctionTool(
            functionName: AgentActivityNames.SubmitReservationAgent,
            functionDescription: "宿泊先の予約を行います。",
            functionParameters: JsonSchemaGenerator.GenerateSchemaAsBinaryData(SourceGenerationContext.Default.HotelReservationRequest)),
        false);

    public static readonly AgentDefinition CommitReservationAgent = new(
        AgentActivityNames.CommitReservationAgent,
        ChatTool.CreateFunctionTool(
            functionName: AgentActivityNames.CommitReservationAgent,
            functionDescription: "宿泊先の予約の確定を行います。",
            functionParameters: JsonSchemaGenerator.GenerateSchemaAsBinaryData(SourceGenerationContext.Default.HotelReservationRequest)),
        true);

    public static AgentDefinition[] AllAgents => new[]
    {
        GetDestinationSuggestAgent,
        GetClimateAgent,
        GetHotelAgent,
        GetSightseeingSpotAgent,
        SubmitReservationAgent,
        CommitReservationAgent
    };

    public AgentDefinition[] GetAgentDefinitions(bool requiresUserConfirmation) =>
        AllAgents.Where(agent => agent.RequiresUserConfirmation == requiresUserConfirmation).ToArray();
}
