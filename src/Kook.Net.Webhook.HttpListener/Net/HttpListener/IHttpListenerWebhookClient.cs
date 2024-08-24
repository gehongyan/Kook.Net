namespace Kook.Net.Webhooks.HttpListener;

internal interface IHttpListenerWebhookClient : IWebhookClient
{
    /// <summary>
    ///     设置此客户端的取消令牌。
    /// </summary>
    /// <param name="cancellationToken"> 要设置的取消令牌。 </param>
    void SetCancellationToken(CancellationToken cancellationToken);

    /// <summary>
    ///     连接到主机。
    /// </summary>
    /// <param name="uriPrefixes"> 要监听的 URI 前缀。 </param>
    /// <returns> 一个表示异步连接操作的任务。 </returns>
    Task StartAsync(IEnumerable<string> uriPrefixes);

    /// <summary>
    ///     断开连接。
    /// </summary>
    /// <returns> 一个表示异步断开连接操作的任务。 </returns>
    Task StopAsync();
}
