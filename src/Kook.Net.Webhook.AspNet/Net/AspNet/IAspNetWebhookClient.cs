using Microsoft.AspNetCore.Http;

namespace Kook.Net.Webhooks.AspNet;

internal interface IAspNetWebhookClient : IWebhookClient
{
    /// <summary>
    ///     处理一个 KOOK Webhook 请求。
    /// </summary>
    /// <param name="httpContext"> KOOK Webhook 请求的 HTTP 上下文。 </param>
    /// <returns> 一个表示异步操作的任务。 </returns>
    Task HandleRequestAsync(HttpContext httpContext);
}
