namespace Kook.Net.Webhooks.HttpListener;

/// <summary>
///     Represents a default <see cref="WebhookProvider"/> that creates <see cref="HttpListenerWebhookClient"/> instances.
/// </summary>
public static class DefaultHttpListenerWebhookProvider
{
    /// <summary>
    ///     A delegate that creates a default <see cref="WebhookProvider"/> instance.
    /// </summary>
    public static readonly WebhookProvider Instance = Create();

    /// <summary>
    ///     Creates a delegate that creates a new <see cref="HttpListenerWebhookClient"/> instance.
    /// </summary>
    /// <returns> A delegate that creates a new <see cref="HttpListenerWebhookClient"/> instance. </returns>
    /// <exception cref="PlatformNotSupportedException">The default WebhookProvider is not supported on this platform.</exception>
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
