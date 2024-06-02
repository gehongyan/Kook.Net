using Microsoft.AspNetCore.Http;

namespace Kook.Net.Webhooks;

internal interface IAspNetWebhookClient : IWebhookClient
{
    /// <summary>
    ///     Handles a Kook webhook request.
    /// </summary>
    /// <param name="httpContext"> The HTTP context to handle the request with. </param>
    /// <returns> A task that represents the asynchronous operation. </returns>
    Task HandleRequestAsync(HttpContext httpContext);
}
