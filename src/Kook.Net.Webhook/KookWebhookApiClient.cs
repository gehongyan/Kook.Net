using System.Text.Json;
using Kook.Net.Rest;
using Kook.Net.WebSockets;

namespace Kook.API;

internal class KookWebhookApiClient : KookSocketApiClient
{
    public KookWebhookApiClient(RestClientProvider restClientProvider, WebSocketProvider webSocketProvider,
        string userAgent, string acceptLanguage, string? url = null,
        RetryMode defaultRetryMode = RetryMode.AlwaysRetry,
        JsonSerializerOptions? serializerOptions = null,
        Func<IRateLimitInfo, Task>? defaultRatelimitCallback = null)
        : base(restClientProvider, webSocketProvider, userAgent,
            acceptLanguage, url, defaultRetryMode, serializerOptions, defaultRatelimitCallback)
    {

    }
}
