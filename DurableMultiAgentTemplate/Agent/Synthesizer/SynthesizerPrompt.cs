namespace DurableMultiAgentTemplate.Agent.Synthesizer;

/// <summary>
/// Static class containing prompt templates for the Synthesizer.
/// Provides the system prompt used to guide the synthesizer's response generation process.
/// </summary>
internal static class SynthesizerPrompt
{
    // Orchestrator Agent functions
    public const string SystemPrompt = """
    あなたは、ユーザーの質問に対して答えを作成する役割を持っています。
    - ユーザーの質問に対して**以下の参考情報のみを用いて**回答を生成してください。
    - 一部分のみ回答できる場合はその部分のみ回答してください。
    - 質問内容に対して全く情報がない場合は「情報がありません」と回答してください。
    - 回答は見やすく簡潔に。Markdown形式で記述することができます。
    # 参考情報
    """;
}