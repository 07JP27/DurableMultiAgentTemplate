using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Azure.AI.OpenAI;
using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var configuration = builder.Configuration;

builder.Services.Configure<AppConfiguration>(configuration.GetSection("AppConfig"));

builder.Services
    .AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddClient<AzureOpenAIClient, AzureOpenAIClientOptions>(options =>
            {
                var endpoint = builder.Configuration["AppConfig:OpenAIEndpoint"];
                if (string.IsNullOrEmpty(endpoint)) throw new InvalidOperationException("AppConfig:OpenAIEndpoint is required.");

                TokenCredential credential = builder.Environment.IsDevelopment() ?
                    new AzureCliCredential() :
                    new DefaultAzureCredential();

                return new AzureOpenAIClient(new Uri(endpoint), credential, options);
            });
        });

builder.Services.Configure<JsonSerializerOptions>(jsonSerializerOptions =>
    {
        jsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        // オーケスとレーター関数のmetadata.SerializedOutputの日本語がエスケープされないように設定
        // https://learn.microsoft.com/ja-jp/dotnet/standard/serialization/system-text-json/character-encoding
        jsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All); 
    });

builder.Build().Run();
