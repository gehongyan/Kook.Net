using System.Text.Json;
using Kook.API;
using Kook.Net.Rest;
using Kook.Net.WebSockets;

namespace Kook.Gateway;

internal class KookGatewayApiClient : KookRestApiClient
{
    public KookGatewayApiClient(RestClientProvider restClientProvider, WebSocketProvider webSocketProvider,
        string userAgent, string acceptLanguage,
        string url = null, RetryMode defaultRetryMode = RetryMode.AlwaysRetry,
        JsonSerializerOptions serializerOptions = null,
        Func<IRateLimitInfo, Task> defaultRatelimitCallback = null)
        : base(restClientProvider, userAgent, acceptLanguage, defaultRetryMode, serializerOptions,
            defaultRatelimitCallback)
    {

    }
}
