using DurableMultiAgentTemplate.Json;
using DurableMultiAgentTemplate.Model;
using OpenAI.Chat;

namespace DurableMultiAgentTemplate.Agent;

//https://learn.microsoft.com/ja-jp/azure/ai-services/openai/how-to/dotnet-migration?tabs=stable
internal class AgentDefinition
{
    public static readonly ChatTool GetDestinationSuggestAgent = ChatTool.CreateFunctionTool(
        functionName: AgentActivityName.GetDestinationSuggestAgent,
        functionDescription: "希望の行き先に求める条件を自然言語で与えると、おすすめの旅行先を提案します。",
        functionParameters: JsonSchemaGenerator.GenerateSchemaAsBinaryData(SourceGenerationContext.Default.GetDestinationSuggestRequest));

    public static readonly ChatTool GetClimateAgent = ChatTool.CreateFunctionTool(
        functionName: AgentActivityName.GetClimateAgent,
        functionDescription: "指定された場所の気候を取得します。",
        functionParameters: JsonSchemaGenerator.GenerateSchemaAsBinaryData(SourceGenerationContext.Default.GetClimateRequest));

    public static readonly ChatTool GetSightseeingSpotAgent = ChatTool.CreateFunctionTool(
        functionName: AgentActivityName.GetSightseeingSpotAgent,
        functionDescription: "指定された場所の観光名所を取得します。",
        functionParameters: JsonSchemaGenerator.GenerateSchemaAsBinaryData(SourceGenerationContext.Default.GetSightseeingSpotRequest));

    public static readonly ChatTool GetHotelAgent = ChatTool.CreateFunctionTool(
        functionName: AgentActivityName.GetHotelAgent,
        functionDescription: "指定された場所のホテルを取得します。",
        functionParameters: JsonSchemaGenerator.GenerateSchemaAsBinaryData(SourceGenerationContext.Default.GetHotelRequest));

    public static readonly ChatTool SubmitReservationAgent = ChatTool.CreateFunctionTool(
        functionName: AgentActivityName.SubmitReservationAgent,
        functionDescription: "宿泊先の予約を行います。",
        functionParameters: JsonSchemaGenerator.GenerateSchemaAsBinaryData(SourceGenerationContext.Default.SubmitReservationRequest));
}
