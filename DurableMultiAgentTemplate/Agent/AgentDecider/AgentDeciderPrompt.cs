namespace DurableMultiAgentTemplate.Agent.AgentDecider;

/// <summary>
/// Static class containing prompt templates for the AgentDecider.
/// Provides the system prompt used to guide the agent's decision-making process.
/// </summary>
internal static class AgentDeciderPrompt
{
    // Orchestrator Agent functions
    public const string SystemPrompt = """
    あなたは、人々が情報を見つけるのを助ける 旅行 AI アシスタントです。
    アシスタントとして、ユーザーからの問いについて必要なツールを選択してください。
    あなたの知識にないことや、使えるツールがない場合は「わかりません」と答えてください。
    使えるツールがあるが、情報が足りない時はユーザーにその情報を質問してください。
    また、旅行以外の話題については答えないでください。
    """;
}