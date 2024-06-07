namespace Kook.Net.Webhooks.HttpListener;

internal interface IHttpListenerWebhookClient : IWebhookClient
{
    /// <summary>
    ///     Sets the cancellation token for this client.
    /// </summary>
    /// <param name="cancellationToken"> The cancellation token to be used. </param>
    void SetCancellationToken(CancellationToken cancellationToken);

    /// <summary>
    ///     Connects to the specified host.
    /// </summary>
    /// <param name="uriPrefixes"> </param>
    /// <returns> A task that represents an asynchronous connect operation. </returns>
    Task StartAsync(IEnumerable<string> uriPrefixes);

    /// <summary>
    ///     Disconnects from the host.
    /// </summary>
    /// <returns> A task that represents an asynchronous disconnect operation. </returns>
    Task StopAsync();
}
