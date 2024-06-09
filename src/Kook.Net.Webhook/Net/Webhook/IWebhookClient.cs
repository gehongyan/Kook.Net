namespace Kook.Net.Webhooks;

/// <summary>
///     Represents a generic WebSocket client.
/// </summary>
public interface IWebhookClient : IDisposable
{
    /// <summary>
    ///     Fired when a binary message is received.
    /// </summary>
    event Func<byte[], int, int, Task<string?>>? BinaryMessage;

    /// <summary>
    ///     Fired when a text message is received.
    /// </summary>
    event Func<string, Task<string?>>? TextMessage;

    /// <summary>
    ///     Fired when the HttpListener is closed.
    /// </summary>
    event Func<Exception, Task>? Closed;

    /// <summary>
    ///     Handles a text message.
    /// </summary>
    /// <param name="requestBody"> The request body. </param>
    /// <returns> A task that represents the asynchronous operation. </returns>
    Task<string?> HandleTextMessageAsync(string requestBody);

    /// <summary>
    ///     Handles a binary message.
    /// </summary>
    /// <param name="data"> The data. </param>
    /// <param name="index"> The index. </param>
    /// <param name="count"> The count. </param>
    /// <returns> A task that represents the asynchronous operation. </returns>
    Task<string?> HandleBinaryMessageAsync(byte[] data, int index, int count);
}
