namespace Kook.Net.Webhooks.HttpListener;

/// <summary>
///     表示一个默认的使用 HTTP 监听器的 <see cref="WebhookProvider"/>，用于创建 <see cref="HttpListenerWebhookClient"/> 实例。
/// </summary>
public static class DefaultHttpListenerWebhookProvider
{
    /// <summary>
    ///     一个创建默认的使用 HTTP 监听器的 <see cref="HttpListenerWebhookClient"/> 实例的委托。
    /// </summary>
    public static readonly WebhookProvider Instance = Create();

    /// <summary>
    ///     创建一个新的用于创建默认的使用 HTTP 监听器的 <see cref="HttpListenerWebhookClient"/> 实例的委托。
    /// </summary>
    /// <returns> 一个用于创建默认的使用 HTTP 监听器的 <see cref="HttpListenerWebhookClient"/> 实例的委托。 </returns>
    /// <exception cref="PlatformNotSupportedException"> 当默认的 <see cref="DefaultHttpListenerWebhookProvider"/> 在当前平台上不受支持时引发。 </exception>
    public static WebhookProvider Create() =>
        () =>
        {
            try
            {
                return new HttpListenerWebhookClient();
            }
            catch (PlatformNotSupportedException ex)
            {
                throw new PlatformNotSupportedException("The default DefaultHttpListenerWebhookProvider is not supported on this platform.", ex);
            }
        };
}
