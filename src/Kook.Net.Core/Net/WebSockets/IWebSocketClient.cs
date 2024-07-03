namespace Kook.Net.WebSockets;

/// <summary>
///     表示一个通用的 WebSocket 客户端。
/// </summary>
public interface IWebSocketClient : IDisposable
{
    /// <summary>
    ///     当接收到二进制消息时触发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:System.Byte"/>[] 参数是接收到的二进制消息的字节数组。 </item>
    ///     <item> <see cref="T:System.Int32"/> 参数是接收到的二进制消息的字节数组的起始位置。 </item>
    ///     <item> <see cref="T:System.Int32"/> 参数是接收到的二进制消息的字节数组的长度。 </item>
    ///     </list>
    /// </remarks>
    event Func<byte[], int, int, Task>? BinaryMessage;

    /// <summary>
    ///     当接收到文本消息时触发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:System.String"/> 参数是接收到的文本消息的字符串。 </item>
    ///     </list>
    /// </remarks>
    event Func<string, Task>? TextMessage;

    /// <summary>
    ///     当 WebSocket 连接关闭时触发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:System.Exception"/> 参数是导致连接关闭的异常。 </item>
    ///     </list>
    /// </remarks>
    event Func<Exception, Task>? Closed;

    /// <summary>
    ///     设置一个将与未来请求一起发送的标头。
    /// </summary>
    /// <param name="key"> 标头的键。 </param>
    /// <param name="value"> 标头的值。 </param>
    void SetHeader(string key, string value);

    /// <summary>
    ///     设置此客户端的保持活动间隔。
    /// </summary>
    /// <param name="keepAliveInterval"> 保持活动间隔。 </param>
    /// <remarks>
    ///     如果未调用此方法，则默认的保持活动间隔是 <see cref="P:System.Net.WebSockets.WebSocket.DefaultKeepAliveInterval"/>。
    /// </remarks>
    /// <seealso cref="P:System.Net.WebSockets.ClientWebSocketOptions.KeepAliveInterval"/>
    void SetKeepAliveInterval(TimeSpan keepAliveInterval);

    /// <summary>
    ///     设置此客户端的取消令牌。
    /// </summary>
    /// <param name="cancellationToken"> 取消令牌。 </param>
    void SetCancellationToken(CancellationToken cancellationToken);

    /// <summary>
    ///     连接到指定的主机。
    /// </summary>
    /// <param name="host"> 要连接的主机。 </param>
    /// <returns> 一个表示异步连接操作的任务。 </returns>
    Task ConnectAsync(string host);

    /// <summary>
    ///    断开与主机的连接。
    /// </summary>
    /// <param name="closeCode"> 要发送给对方主机的关闭代码。 </param>
    /// <returns> 一个表示异步断开操作的任务。 </returns>
    Task DisconnectAsync(int closeCode = 1000);

    /// <summary>
    ///     向对方主机发送文本消息。
    /// </summary>
    /// <param name="data"> 要发送的数据。 </param>
    /// <param name="index"> 要发送的数据的起始位置。 </param>
    /// <param name="count"> 要发送的数据的长度。 </param>
    /// <param name="isText"> 发送的是否为文本消息。 </param>
    /// <returns> 一个表示异步发送操作的任务。 </returns>
    Task SendAsync(byte[] data, int index, int count, bool isText);
}
