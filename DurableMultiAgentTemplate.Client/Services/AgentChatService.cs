using DurableMultiAgentTemplate.Model;

namespace DurableMultiAgentTemplate.Client.Services;

public class AgentChatService(HttpClient httpClient)
{
    public async Task<AgentResponseWithAdditionalInfoDto> GetAgentResponseAsync(AgentRequestDto agentRequestDto)
    {
        var response = await httpClient.PostAsJsonAsync("invoke/sync", agentRequestDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AgentResponseWithAdditionalInfoDto>() ??
            throw new InvalidOperationException("Agent からのレスポンスのフォーマットが不正です。");
    }
}
