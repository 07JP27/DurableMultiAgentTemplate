using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Azure;
using Azure.AI.OpenAI;
using Azure.Identity;
using DurableMultiAgentTemplate.Agent.Workers;
using DurableMultiAgentTemplate.Model;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var configuration = builder.Configuration;

builder.Services.Configure<AppConfig>(configuration.GetSection("AppConfig"));
builder.Services.AddSingleton<AgentDefinitions>();

builder.Services
    .AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddClient<AzureOpenAIClient, AzureOpenAIClientOptions>(static (options, credential, provider) =>
            {
                var appConfig = provider.GetRequiredService<IOptions<AppConfig>>().Value;
                if (string.IsNullOrEmpty(appConfig.OpenAI.Endpoint)) 
                    throw new InvalidOperationException("AppConfig:OpenAI:Endpoint is required.");

                return string.IsNullOrWhiteSpace(appConfig.OpenAI.ApiKey) ?
                    new AzureOpenAIClient(new Uri(appConfig.OpenAI.Endpoint), credential, options) :
                    new AzureOpenAIClient(new Uri(appConfig.OpenAI.Endpoint), new AzureKeyCredential(appConfig.OpenAI.ApiKey), options);
            }).ConfigureOptions(builder.Configuration.GetSection("AppConfig:OpenAI"));

            clientBuilder.AddClient<CosmosClient, CosmosClientOptions>(static (options, credential, provider) =>
            {
                var appConfig = provider.GetRequiredService<IOptions<AppConfig>>().Value;
                if (string.IsNullOrEmpty(appConfig.CosmosDb.Endpoint))
                    throw new InvalidOperationException("AppConfig:CosmosDb:Endpoint is required.");

                return string.IsNullOrWhiteSpace(appConfig.CosmosDb.ApiKey) ?
                    new CosmosClient(appConfig.CosmosDb.Endpoint, credential, options) :
                    new CosmosClient(appConfig.CosmosDb.Endpoint, appConfig.CosmosDb.ApiKey, options);
            }).ConfigureOptions(builder.Configuration.GetSection("AppConfig:CosmosDb"));

            clientBuilder.UseCredential(builder.Environment.IsDevelopment() ?
                new AzureCliCredential() :
                new DefaultAzureCredential());
        });

builder.Services.AddSingleton(sp =>
{
    var appConfiguration = sp.GetRequiredService<IOptions<AppConfig>>().Value;
    _ = appConfiguration.OpenAI.ChatModelDeployName ?? throw new InvalidOperationException("AppConfig:OpenAI:ChatModelDeployName is required.");
    var openAIClient = sp.GetRequiredService<AzureOpenAIClient>();
    return openAIClient.GetChatClient(appConfiguration.OpenAI.ChatModelDeployName);
});
builder.Services.AddSingleton(sp =>
{ 
    var appConfiguration = sp.GetRequiredService<IOptions<AppConfig>>().Value;
    _ = appConfiguration.OpenAI.EmbeddingModelDeployName ?? throw new InvalidOperationException("AppConfig:OpenAI:EmbeddingModelDeployName is required.");
    var openAIClient = sp.GetRequiredService<AzureOpenAIClient>();
    return openAIClient.GetEmbeddingClient(appConfiguration.OpenAI.EmbeddingModelDeployName);
});

builder.Services.Configure<JsonSerializerOptions>(jsonSerializerOptions =>
    {
        jsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        // オーケスとレーター関数のmetadata.SerializedOutputの日本語がエスケープされないように設定
        // https://learn.microsoft.com/ja-jp/dotnet/standard/serialization/system-text-json/character-encoding
        jsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All); 
    });

builder.Build().Run();
