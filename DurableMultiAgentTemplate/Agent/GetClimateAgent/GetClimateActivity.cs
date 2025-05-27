using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;

namespace DurableMultiAgentTemplate.Agent.GetClimateAgent;

/// <summary>
/// Activity class responsible for retrieving climate information for a specified location.
/// Part of the multi-agent travel concierge system.
/// </summary>
public class GetClimateActivity(ChatClient chatClient, 
    ILogger<GetClimateActivity> logger)
{
    [Function(AgentActivityName.GetClimateAgent)]
    public async Task<string> RunAsync([ActivityTrigger] GetClimateRequest req)
    {
        if(Random.Shared.Next(0, 10) < 3)
        {
            logger.LogInformation("Failed to get climate information");
            throw new InvalidOperationException("Failed to get climate information");
        }

        // Simulate a delay
        await Task.Delay(3000);

        // This is sample code. Replace this with your own logic.
        var result = $"""
        {req.Location}の気候は年間を通じて暖かく、**熱帯モンスーン気候**に分類されます。大きく分けて**乾季**と**雨季**があり、それぞれ異なる特徴があります。
        ---

        ### 平均気温
        - **年間を通じて：** 26～30℃程度
        - **日中：** 30℃前後まで上がることが多い。
        - **夜間：** 23～25℃程度で過ごしやすい。

        ---

        ### 乾季（5月～10月）
        - **特徴：**
        - 晴れの日が多く、湿度が比較的低い。
        - 海や観光に最適なシーズン。
        - 朝晩は涼しい風が吹き、快適に過ごせる。
        - **おすすめのアクティビティ：**
        - ビーチでのリラックス
        - ダイビングやサーフィン
        - ウブド周辺でのトレッキングや文化体験

        ---

        ### 雨季（11月～4月）
        - **特徴：**
        - 短時間のスコールが頻繁に発生。
        - 湿度が高く蒸し暑い。
        - 雨が降ってもその後すぐに晴れることが多い。
        - **おすすめのアクティビティ：**
        - 室内スパやリゾート内でのリラクゼーション
        - ヒンズー寺院巡りや地元の文化体験
        - 雨季特有の緑が豊かな景色を楽しむ

        ---

        ### 服装のポイント
        - **乾季：** 半袖や軽い素材の服装でOK。朝晩の冷えに備えて薄手の上着を用意。
        - **雨季：** 雨具（折りたたみ傘やレインコート）があると便利。速乾性の服装がおすすめ。

        ---

        {req.Location}は雨季でも旅行を楽しめるよう工夫されているため、いつ訪れても魅力的です。乾季の5月～10月が観光のベストシーズンとされていますが、雨季なら緑豊かな景観と比較的空いている観光地を楽しむことができます。
        """;

        return result;
    }
}
