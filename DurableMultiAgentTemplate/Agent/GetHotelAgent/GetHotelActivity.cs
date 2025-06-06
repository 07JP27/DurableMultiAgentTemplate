﻿using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;

namespace DurableMultiAgentTemplate.Agent.GetHotelAgent;

/// <summary>
/// Activity class responsible for retrieving hotel information for a specified location.
/// Part of the multi-agent travel concierge system.
/// </summary>
public class GetHotelActivity(ChatClient chatClient, 
    ILogger<GetHotelActivity> logger)
{
    /// <summary>
    /// Retrieves hotel information for the specified location.
    /// Simulates a network call with potential failures and returns hotel recommendations.
    /// </summary>
    /// <param name="req">Request containing the location for which to retrieve hotel information</param>
    /// <returns>Detailed hotel recommendations for the specified location in Japanese</returns>
    [Function(AgentActivityName.GetHotelAgent)]
    public async Task<string> Run([ActivityTrigger] GetHotelRequest req)
    {
        if (Random.Shared.Next(0, 10) < 3)
        {
            logger.LogInformation("Failed to get hotel information");
            throw new InvalidOperationException("Failed to get hotel information");
        }

        // Simulate a delay
        await Task.Delay(3000);
        // This is sample code. Replace this with your own logic.
        var result = $"""
        {req.Location}に以下の４件のホテルがあります。
        ---

        ### 1. **リラ・オアシスホテル（Rila Oasis Hotel）**
        - **所在地**: 海沿いのリゾート地
        - **テーマ**: 癒しとウェルネス
        - **特徴**:  
        - 海を望むインフィニティプール  
        - スパ・ヨガ・瞑想などのウェルネスプログラム  
        - 地元食材を使ったヘルシーレストラン  
        - 落ち着いた内装でリラクゼーションを重視  

        ---

        ### 2. **クラウン・スカイタワー（Crown Skytower）**
        - **所在地**: 都市部の中心地
        - **テーマ**: モダンでラグジュアリーな都市体験
        - **特徴**:  
        - 高層階からの夜景が楽しめるラグジュアリールーム  
        - 最先端の設備を備えたビジネスセンター  
        - ミシュラン星付きレストラン併設  
        - スタイリッシュなデザインとスマートホテル機能  

        ---

        ### 3. **フォレスト・ヒドゥンロッジ（Forest Hidden Lodge）**
        - **所在地**: 森に囲まれた山間部
        - **テーマ**: 自然と共に過ごす冒険と安らぎ
        - **特徴**:  
        - 木造のコテージ風宿泊施設  
        - トレッキングや星空観察のアクティビティ  
        - 暖炉付きラウンジと温泉  
        - 地元の伝統料理を楽しめるダイニング  

        ---

        ### 4. **アルテ・シンフォニア（Arte Sinfonia）**
        - **所在地**: 歴史的な街並みの一角
        - **テーマ**: アートと文化の融合
        - **特徴**:  
        - 地元アーティストの作品を展示したギャラリー併設  
        - クラシック音楽のライブ演奏が行われるラウンジ  
        - アンティーク家具を取り入れたクラシカルな内装  
        - 歴史的建築物をリノベーションした趣のある空間  

        ---
        """;

        return result;
    }
}
