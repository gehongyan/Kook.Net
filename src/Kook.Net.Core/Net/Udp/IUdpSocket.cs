namespace Kook.Net.Udp;

/// <summary>
///     Represents a generic UDP socket.
/// </summary>
public interface IUdpSocket : IDisposable
{
    /// <summary>
    ///     Fired when a datagram is received.
    /// </summary>
    event Func<byte[], int, int, Task> ReceivedDatagram;

    /// <summary>
    ///     Gets the port of the socket.
    /// </summary>
    ushort Port { get; }

    /// <summary>
    ///     Sets the cancellation token.
    /// </summary>
    /// <param name="cancellationToken"> The cancellation token. </param>
    void SetCancellationToken(CancellationToken cancellationToken);

    /// <summary>
    ///     Sets the destination of the socket.
    /// </summary>
    /// <param name="ip"> The IP address of the destination. </param>
    /// <param name="port"> The port of the destination. </param>
    void SetDestination(string ip, int port);

    /// <summary>
    ///     Starts the socket.
    /// </summary>
    /// <returns> A task that represents the asynchronous operation. </returns>
    Task StartAsync();

    /// <summary>
    ///     Stops the socket.
    /// </summary>
    /// <returns> A task that represents the asynchronous operation. </returns>
    Task StopAsync();

    /// <summary>
    ///     Sends a datagram.
    /// </summary>
    /// <param name="data"> The data to send. </param>
    /// <param name="index"> The index of the data to start sending from. </param>
    /// <param name="count"> The number of bytes to send. </param>
    /// <returns> A task that represents the asynchronous operation. </returns>
    Task SendAsync(byte[] data, int index, int count);
}
