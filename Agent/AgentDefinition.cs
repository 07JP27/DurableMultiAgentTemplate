using OpenAI.Chat;

namespace DurableMultiAgentTemplate
{
    //https://learn.microsoft.com/ja-jp/azure/ai-services/openai/how-to/dotnet-migration?tabs=stable
    internal class AgentDefinition
    {
        public static readonly ChatTool GetDestinationSuggestAgent = ChatTool.CreateFunctionTool(
            functionName: AgentActivityName.GetDestinationSuggestAgent,
            functionDescription: "希望の行き先に求める条件を自然言語で与えると、おすすめの旅行先を提案します。",
            functionParameters: BinaryData.FromString("""
            {
                "type": "object",
                "properties": {
                    "searchTerm": {
                        "type": "string",
                        "description": "行き先に求める希望の条件"
                    }
                },
                "required": [ "searchTerm" ]
            }
            """)
        );

        public static readonly ChatTool GetClimateAgent = ChatTool.CreateFunctionTool(
            functionName: AgentActivityName.GetClimateAgent,
            functionDescription: "指定された場所の気候を取得します。",
            functionParameters: BinaryData.FromString("""
            {
                "type": "object",
                "properties": {
                    "location": {
                        "type": "string",
                        "description": "場所の名前。例: ボストン, 東京、フランス"
                    }
                },
                "required": [ "location" ]
            }
            """)
        );

        public static readonly ChatTool GetSightseeingSpotAgent = ChatTool.CreateFunctionTool(
            functionName: AgentActivityName.GetSightseeingSpotAgent,
            functionDescription: "指定された場所の観光名所を取得します。",
            functionParameters: BinaryData.FromString("""
            {
                "type": "object",
                "properties": {
                    "location": {
                        "type": "string",
                        "description": "場所の名前。例: ボストン, 東京、フランス"
                    }
                },
                "required": [ "location" ]
            }
            """)
        );
        public static readonly ChatTool GetHotelAgent = ChatTool.CreateFunctionTool(
            functionName: AgentActivityName.GetHotelAgent,
            functionDescription: "指定された場所のホテルを取得します。",
            functionParameters: BinaryData.FromString("""
            {
                "type": "object",
                "properties": {
                    "location": {
                        "type": "string",
                        "description": "場所の名前。例: ボストン, 東京、フランス"
                    }
                },
                "required": [ "location" ]
            }
            """)
        );

        public static readonly ChatTool SubmitReservationAgent = ChatTool.CreateFunctionTool(
            functionName: AgentActivityName.SubmitReservationAgent,
            functionDescription: "宿泊先の予約を行います。",
            functionParameters: BinaryData.FromString("""
            {
                "type": "object",
                "properties": {
                    "destination": {
                        "type": "string",
                        "description": "行き先のホテルの名前。"
                    },
                    "checkIn": {
                        "type": "string",
                        "description": "チェックイン日。YYYY/MM/DD形式。"
                    },
                    "checkOut": {
                        "type": "string",
                        "description": "チェックアウト日。YYYY/MM/DD形式。"
                    },
                    "guestsCount": {
                        "type": "integer",
                        "description": "宿泊人数。"
                    }
                },
                "required": [ "destination", "checkIn", "checkOut", "guests" ]
            }
            """)
        );
    }
}