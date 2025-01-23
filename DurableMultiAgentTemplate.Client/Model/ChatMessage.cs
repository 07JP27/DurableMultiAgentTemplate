using DurableMultiAgentTemplate.Model;

namespace DurableMultiAgentTemplate.Client.Model;

public abstract record ChatMessage(Role Role, bool IsRequestTarget);
public record UserChatMessage(string Message) : ChatMessage(Role.User, true);
public record AgentChatMessage(AgentResponseWithAdditionalInfoDto Message) : ChatMessage(Role.Assistant, true);
public record InfoChatMessage(string Info, bool IsShowProgress) : ChatMessage(Role.Info, false);

public enum Role
{
    User,
    Assistant,
    Info
}

public static class RoleExtensions
{
    public static string ToRoleName(this Role role) => role switch
    {
        Role.User => "user",
        Role.Assistant => "assistant",
        _ => throw new InvalidOperationException()
    };
}
