namespace Kook.Net.Webhooks.AspNet;

/// <summary>
///     表示一个默认的使用 ASP.NET 的 <see cref="WebhookProvider"/>，用于创建 <see cref="AspNetWebhookClient"/> 实例。
/// </summary>
public static class DefaultAspNetWebhookProvider
{
    /// <summary>
    ///     一个创建默认的使用 ASP.NET 的 <see cref="AspNetWebhookClient"/> 实例的委托。
    /// </summary>
    public static readonly WebhookProvider Instance = Create();

    /// <summary>
    ///     创建一个新的用于创建默认的使用 ASP.NET 的 <see cref="AspNetWebhookClient"/> 实例的委托。
    /// </summary>
    /// <returns> 一个用于创建默认的使用 ASP.NET 的 <see cref="AspNetWebhookClient"/> 实例的委托。 </returns>
    /// <exception cref="PlatformNotSupportedException"> 当默认的 <see cref="DefaultAspNetWebhookProvider"/> 在当前平台上不受支持时引发。 </exception>
    public static WebhookProvider Create() =>
        () =>
        {
            try
            {
                return new AspNetWebhookClient();
            }
            catch (PlatformNotSupportedException ex)
            {
                throw new PlatformNotSupportedException("The default DefaultAspNetWebhookProvider is not supported on this platform.", ex);
            }
        };
}
