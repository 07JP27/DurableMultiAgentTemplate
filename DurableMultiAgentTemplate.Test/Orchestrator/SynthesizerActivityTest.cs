using System.ClientModel;
using System.ClientModel.Primitives;
using DurableMultiAgentTemplate.Agent;
using DurableMultiAgentTemplate.Agent.Orchestrator;
using DurableMultiAgentTemplate.Agent.Synthesizer;
using DurableMultiAgentTemplate.Model;
using DurableMultiAgentTemplate.Shared.Model;
using Microsoft.Extensions.Logging;
using Moq;
using OpenAI.Chat;

namespace DurableMultiAgentTemplate.Tests.Orchestrator;

[TestClass]
public class SynthesizerActivityTest
{
    [TestMethod]
    public async Task SynthesizerMethod()
    {
        var expectedContent = "バリ島がおすすめです！";
        var chatCompletion = OpenAIChatModelFactory.ChatCompletion(
            content: new ChatMessageContent(expectedContent),
            finishReason: ChatFinishReason.Stop
        );
        var pipelineResponseMock = Mock.Of<PipelineResponse>();

        var chatClientMock = new Mock<ChatClient>();
        chatClientMock
            .Setup(
                x => x.CompleteChatAsync(It.IsAny<ChatMessage[]>())
            )
            .ReturnsAsync(ClientResult.FromValue(chatCompletion, pipelineResponseMock));

        var loggerMock = new Mock<ILogger<SynthesizerActivity>>();
        var synthesizerActivity = new SynthesizerActivity(chatClientMock.Object,loggerMock.Object);
        var synthesizerRequest = new SynthesizerRequest { 
            AgentCallResult =["""
            暖かい場所の条件でおすすめの旅行先を提案します。好みに応じて選んでください。
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
            """],
            AgentRequest = new AgentRequestDto {
                Messages = new List<AgentRequestMessageItem> { new AgentRequestMessageItem { Role = "user", Content = "あったかい場所に行きたいな" } },
            },
            CalledAgentNames = new List<string> { AgentActivityName.GetDestinationSuggestAgent }
        };

        var agentResponseDto = await synthesizerActivity.Run(synthesizerRequest);
        
        Assert.IsNotNull(agentResponseDto);
        Assert.IsNotEmpty(agentResponseDto.Content);
        Assert.AreEqual(agentResponseDto.Content, expectedContent);
        Assert.AreEqual(synthesizerRequest.CalledAgentNames, agentResponseDto.CalledAgentNames);
    }
}
