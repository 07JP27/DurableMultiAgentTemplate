using Azure.AI.OpenAI;
using DurableMultiAgentTemplate.Model;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DurableMultiAgentTemplate.Agent.GetSightseeingSpotAgent;

public class GetSightseeingSpotActivity(AzureOpenAIClient openAIClient, 
    IOptions<AppConfiguration> configuration,
    ILogger<GetSightseeingSpotActivity> logger)
{
    private readonly AzureOpenAIClient _openAIClient = openAIClient;
    private readonly AppConfiguration _configuration = configuration.Value;

    [Function(AgentActivityName.GetSightseeingSpotAgent)]
    public string Run([ActivityTrigger] GetSightseeingSpotRequest req, FunctionContext executionContext)
    {
        if (Random.Shared.Next(0, 10) < 3)
        {
            logger.LogInformation("Failed to get sightseeing spot information");
            throw new InvalidOperationException("Failed to get sightseeing spot information");
        }

        // This is sample code. Replace this with your own logic.
        var result = $"""
        {req.Location}には美しい自然、歴史的な寺院、ユニークな文化体験が楽しめる観光名所がたくさんあります！以下におすすめの観光スポットをまとめました。

        ---

        ### 1. **寺院**
        #### **タナロット寺院（Pura Tanah Lot）**
        - 海の上に建つ寺院で、夕日とともに見える景色が絶景。
        - {req.Location}を代表する観光名所の一つ。

        #### **ウルワツ寺院（Pura Luhur Uluwatu）**
        - 崖の上に位置する寺院。インド洋を見渡す絶景スポット。
        - 夕方には伝統舞踊「ケチャダンス」の公演が行われる。

        #### **ティルタ・エンプル寺院（Pura Tirta Empul）**
        - 聖水が湧き出る寺院で、地元の人々や観光客が浄化の儀式を体験。
        - 神聖な雰囲気を感じることができる。

        ---

        ### 2. **自然**
        #### **ウブドのライステラス（Tegalalang Rice Terrace）**
        - 階段状に広がる田んぼの風景が特徴的。
        - 写真撮影やトレッキングに最適。

        #### **モンキーフォレスト（Sacred Monkey Forest Sanctuary）**
        - ウブドにある野生の猿が暮らす自然保護区。
        - 緑豊かな森と寺院が融合した神秘的な場所。

        #### **キンタマーニ高原とバトゥール山**
        - バトゥール火山とカルデラ湖の壮大な景色を楽しめる。
        - トレッキングや朝日鑑賞がおすすめ。

        ---

        ### 3. **ビーチ**
        #### **クタビーチ**
        - サーフィン初心者に人気のスポット。
        - 多くのレストランやショップが近くにあり、賑やかな雰囲気。

        #### **ヌサ・ドゥア**
        - 高級リゾートエリアで、静かなビーチと透明度の高い海が魅力。
        - シュノーケリングやジェットスキーなどのマリンスポーツも楽しめる。

        #### **ジンバランビーチ**
        - サンセットディナーが有名で、新鮮なシーフード料理が楽しめる。
        - 夕日を眺めながらの食事は特別な体験。

        ---

        ### 4. **文化体験**
        #### **ウブド王宮（Ubud Palace）**
        - 伝統的な建築が美しい王宮で、舞踊のパフォーマンスが行われる。
        - ウブド市場も近くにあり、ショッピングにも最適。

        #### **伝統舞踊**
        - ケチャダンスやレゴンダンスなど、寺院や専用会場で観賞できる。
        - 神話や伝説が題材となった迫力あるパフォーマンス。

        ---

        ### 5. **アクティビティ**
        #### **サファリ＆マリンパーク**
        - 動物園や水族館が融合したエンターテイメント施設。
        - サファリツアーやアニマルショーが楽しめる。

        #### **ラフティング（アユン川）**
        - 緑に囲まれた川を下る冒険感あふれる体験。
        - 初心者でも楽しめるコースが多い。

        ---

        ### 6. **近隣の島**
        #### **ヌサペニダ島**
        - {req.Location}から船でアクセス可能な離島。
        - クリスタルベイやケリンキンビーチの絶景が有名。

        #### **レンボンガン島**
        - マングローブツアーやサンゴ礁シュノーケリングが楽しめる。
        - のんびりとした雰囲気の島。

        ---

        {req.Location}は多様な楽しみ方ができるため、目的に応じて行き先を選んでみてください！
        """;

        return result;
    }
}
