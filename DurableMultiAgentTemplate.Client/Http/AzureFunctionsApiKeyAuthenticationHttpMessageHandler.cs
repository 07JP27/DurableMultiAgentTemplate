using Microsoft.Extensions.Options;

namespace DurableMultiAgentTemplate.Client.Http;

/// <summary>
/// HTTP message handler that adds Azure Functions API key authentication to requests.
/// Implements the delegating handler pattern to inject authentication headers.
/// </summary>
public class AzureFunctionsApiKeyAuthenticationHttpMessageHandler(IOptions<BackendOptions> backendOptions) : DelegatingHandler
{
    /// <summary>
    /// Sends an HTTP request to the inner handler after adding the API key header if configured.
    /// </summary>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The HTTP response message.</returns>
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(backendOptions.Value.ApiKey))
        {
            request.Headers.Add("x-functions-key", backendOptions.Value.ApiKey);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
