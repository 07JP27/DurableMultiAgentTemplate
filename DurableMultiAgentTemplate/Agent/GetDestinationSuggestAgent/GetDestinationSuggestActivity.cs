﻿using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;

namespace DurableMultiAgentTemplate.Agent.GetDestinationSuggestAgent;

/// <summary>
/// Activity class responsible for suggesting travel destinations based on user preferences.
/// Provides recommendations for both domestic and international locations.
/// </summary>
public class GetDestinationSuggestActivity(ChatClient chatClient,
    ILogger<GetDestinationSuggestActivity> logger)
{
    /// <summary>
    /// Generates travel destination suggestions based on the provided search criteria.
    /// Simulates a network call with potential failures and returns categorized destination recommendations.
    /// </summary>
    /// <param name="req">Request containing search terms and criteria for destination suggestions</param>
    /// <returns>Categorized destination suggestions (domestic and international) in Japanese</returns>
    [Function(AgentActivityName.GetDestinationSuggestAgent)]
    public async Task<string> RunAsync([ActivityTrigger] GetDestinationSuggestRequest req)
    {
        if (Random.Shared.Next(0, 10) < 3)
        {
            logger.LogInformation("Failed to get destination suggestions");
            throw new InvalidOperationException("Failed to get destination suggestions");
        }

        // Simulate a delay
        await Task.Delay(3000);

        // This is sample code. Replace this with your own logic.
        var result = $"""
        {req.SearchTerm}の条件でおすすめの旅行先を提案します。好みに応じて選んでください。
        ### 国内
        1. **沖縄本島**  
        - 透明度の高いビーチ、首里城、美ら海水族館など観光名所が豊富。
        - 冬でも暖かく、リラックスした雰囲気を楽しめる。

        2. **石垣島・宮古島**  
        - 南国らしい美しい自然が広がり、ダイビングやシュノーケリングが人気。
        - 島ならではの郷土料理も楽しめる。

        3. **鹿児島・奄美大島**  
        - 奄美の黒糖焼酎や島唄、特有の自然環境を満喫。
        - 亜熱帯の雰囲気を楽しめる。

        ---

        ### 海外
        1. **ハワイ（オアフ島やマウイ島）**  
        - 年間を通して快適な気温。ビーチリゾートやトレッキングなど多様なアクティビティが可能。
        - 日本語対応も充実していて安心。

        2. **タイ（プーケットやクラビ）**  
        - 手頃な価格で楽しめるリゾート地。温かい気候とタイ料理も魅力。
        - 観光名所や島巡りもおすすめ。

        3. **バリ島（インドネシア）**  
        - 高級リゾートから手軽な宿泊施設まで選択肢が広い。
        - ヒンズー文化と自然が織りなすユニークな雰囲気を堪能。

        4. **オーストラリア（ケアンズやゴールドコースト）**  
        - グレートバリアリーフでの海洋アクティビティが人気。
        - 暖かい気候で自然と都市観光をバランス良く楽しめる。

        5. **グアムやサイパン**  
        - 日本から近く、短期間でも楽しめる南国リゾート。
        - のんびり過ごしたい方に最適。

        どれも暖かい気候を楽しめる場所です。予算や旅行期間に合わせてお選びください！
        """;

        return result;
    }
}
