using Microsoft.Extensions.Options;

namespace DurableMultiAgentTemplate.Client.Http;

public class AzureFunctionsApiKeyAuthenticationHttpMessageHandler(IOptions<BackendOptions> backendOptions) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(backendOptions.Value.ApiKey))
        {
            request.Headers.Add("x-functions-key", backendOptions.Value.ApiKey);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
