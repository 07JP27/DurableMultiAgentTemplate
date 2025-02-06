namespace DurableMultiAgentTemplate.Model;

public class AppConfig
{
    public required OpenAIConfig OpenAI { get; init; }
    public required CosmosDbConfig CosmosDb { get; init; }
}

public class OpenAIConfig
{
    public required string Endpoint { get; init; }
    public string? ApiKey { get; init; }
    public required string Deploy { get; init; }
}

public class CosmosDbConfig
{
    public required string Endpoint { get; init; }
    public string? ApiKey { get; init; }
}
