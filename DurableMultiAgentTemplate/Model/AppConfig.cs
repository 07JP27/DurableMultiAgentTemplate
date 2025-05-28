namespace DurableMultiAgentTemplate.Model;

/// <summary>
/// Configuration class for the application.
/// Contains settings for OpenAI and Cosmos DB services.
/// </summary>
public class AppConfig
{
    /// <summary>
    /// Gets or initializes the OpenAI configuration.
    /// </summary>
    public required OpenAIConfig OpenAI { get; init; }
    
    /// <summary>
    /// Gets or initializes the Cosmos DB configuration.
    /// </summary>
    public required CosmosDbConfig CosmosDb { get; init; }
}

/// <summary>
/// Configuration class for OpenAI services.
/// Includes endpoint, API key, and model deployment names.
/// </summary>
public class OpenAIConfig
{
    /// <summary>
    /// Gets or initializes the endpoint URL for the OpenAI API.
    /// </summary>
    public required string Endpoint { get; init; }
    
    /// <summary>
    /// Gets or initializes the API key for authentication with OpenAI services.
    /// </summary>
    public string? ApiKey { get; init; }
    
    /// <summary>
    /// Gets or initializes the deployment name for the chat model.
    /// </summary>
    public required string ChatModelDeployName { get; init; }
    
    /// <summary>
    /// Gets or initializes the deployment name for the embedding model.
    /// </summary>
    public required string EmbeddingModelDeployName { get; init; }
}

/// <summary>
/// Configuration class for Cosmos DB.
/// Includes endpoint and API key settings.
/// </summary>
public class CosmosDbConfig
{
    /// <summary>
    /// Gets or initializes the endpoint URL for Cosmos DB.
    /// </summary>
    public required string Endpoint { get; init; }
    
    /// <summary>
    /// Gets or initializes the API key for authentication with Cosmos DB.
    /// </summary>
    public string? ApiKey { get; init; }
}
