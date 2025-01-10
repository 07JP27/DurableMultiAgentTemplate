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

var config = new AppConfiguration();
configuration.GetSection("AppConfig").Bind(config);
builder.Services.AddSingleton(config);

builder.Services
    .AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddClient<AzureOpenAIClient, AzureOpenAIClientOptions>(options =>
            {
                var endpoint = config.OpenAIEndpoint;
                if (string.IsNullOrEmpty(endpoint)) throw new InvalidOperationException("AppConfig:OpenAIEndpoint is required.");

                TokenCredential credential = builder.Environment.IsDevelopment() ?
                    new AzureCliCredential() :
                    new DefaultAzureCredential();

                return new AzureOpenAIClient(new Uri(endpoint), credential, options);
            });
        });

builder.Build().Run();