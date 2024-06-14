namespace Kook.Net.Webhooks.AspNet;

/// <summary>
///     Represents a default <see cref="WebhookProvider"/> that creates <see cref="AspNetWebhookClient"/> instances.
/// </summary>
public static class DefaultAspNetWebhookProvider
{
    /// <summary>
    ///     A delegate that creates a default <see cref="WebhookProvider"/> instance.
    /// </summary>
    public static readonly WebhookProvider Instance = Create();

    /// <summary>
    ///     Creates a delegate that creates a new <see cref="AspNetWebhookClient"/> instance.
    /// </summary>
    /// <returns> A delegate that creates a new <see cref="AspNetWebhookClient"/> instance. </returns>
    /// <exception cref="PlatformNotSupportedException">The default WebhookProvider is not supported on this platform.</exception>
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
