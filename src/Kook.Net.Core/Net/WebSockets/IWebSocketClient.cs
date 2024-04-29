namespace Kook.Net.WebSockets;

/// <summary>
///     Represents a generic WebSocket client.
/// </summary>
public interface IWebSocketClient : IDisposable
{
    /// <summary>
    ///     Fired when a binary message is received.
    /// </summary>
    event Func<byte[], int, int, Task>? BinaryMessage;

    /// <summary>
    ///     Fired when a text message is received.
    /// </summary>
    event Func<string, Task>? TextMessage;

    /// <summary>
    ///     Fired when the WebSocket connection is closed.
    /// </summary>
    event Func<Exception, Task>? Closed;

    /// <summary>
    ///     Sets a header to be sent with the future requests.
    /// </summary>
    /// <param name="key"> The field name of the header. </param>
    /// <param name="value"> The value of the header. </param>
    void SetHeader(string key, string value);

    /// <summary>
    ///     Sets the keep-alive interval for this client.
    /// </summary>
    /// <param name="keepAliveInterval"> The keep-alive interval to be used. </param>
    /// <remarks>
    ///     If this method is not called, the default keep-alive interval will be
    ///     <see cref="P:System.Net.WebSockets.WebSocket.DefaultKeepAliveInterval"/>.
    /// </remarks>
    void SetKeepAliveInterval(TimeSpan keepAliveInterval);

    /// <summary>
    ///     Sets the cancellation token for this client.
    /// </summary>
    /// <param name="cancellationToken"> The cancellation token to be used. </param>
    void SetCancellationToken(CancellationToken cancellationToken);

    /// <summary>
    ///     Connects to the specified host.
    /// </summary>
    /// <param name="host"> The host to connect to. </param>
    /// <returns> A task that represents an asynchronous connect operation. </returns>
    Task ConnectAsync(string host);

    /// <summary>
    ///     Disconnects from the host.
    /// </summary>
    /// <param name="closeCode"> The close code to be sent to the host. </param>
    /// <returns> A task that represents an asynchronous disconnect operation. </returns>
    Task DisconnectAsync(int closeCode = 1000);

    /// <summary>
    ///     Sends a message to the host.
    /// </summary>
    /// <param name="data"> The data to be sent. </param>
    /// <param name="index"> The index of the data to start sending from. </param>
    /// <param name="count"> The amount of data to send. </param>
    /// <param name="isText"> Whether the data is text or binary. </param>
    /// <returns> A task that represents an asynchronous send operation. </returns>
    Task SendAsync(byte[] data, int index, int count, bool isText);
}
