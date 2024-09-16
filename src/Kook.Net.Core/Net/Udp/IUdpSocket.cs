namespace Kook.Net.Udp;

/// <summary>
///     表示一个通用的 UDP 套接字。
/// </summary>
public interface IUdpSocket : IDisposable
{
    /// <summary>
    ///     当接收到数据报时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="System.Byte"/>[] 参数是接收到的数据报的字节数组。 </item>
    ///     <item> <see cref="System.Int32"/> 参数是接收到的数据报的字节数组的起始位置。 </item>
    ///     <item> <see cref="System.Int32"/> 参数是接收到的数据报的字节数组的长度。 </item>
    ///     </list>
    /// </remarks>
    event Func<byte[], int, int, Task>? ReceivedDatagram;

    /// <summary>
    ///     获取 UDP 套接字的端口号。
    /// </summary>
    ushort Port { get; }

    /// <summary>
    ///     设置此客户端的取消令牌。
    /// </summary>
    /// <param name="cancellationToken"> 用于取消任务的取消令牌。 </param>
    void SetCancellationToken(CancellationToken cancellationToken);

    /// <summary>
    ///     设置 UDP 套接字的目标通信端点，包括 IP 地址和端口号。
    /// </summary>
    /// <param name="ip"> 目标通信端点的 IP 地址。 </param>
    /// <param name="port"> 目标通信端点的端口号。 </param>
    void SetDestination(string ip, int port);

    /// <summary>
    ///     启动套接字。
    /// </summary>
    /// <returns> 一个表示异步启动操作的任务。 </returns>
    Task StartAsync();

    /// <summary>
    ///     停止套接字。
    /// </summary>
    /// <returns> 一个表示异步停止操作的任务。 </returns>
    Task StopAsync();

    /// <summary>
    ///     发送数据报。
    /// </summary>
    /// <param name="data"> 要发送的数据报的字节数组。 </param>
    /// <param name="index"> 要发送的数据报的字节数组的起始位置。 </param>
    /// <param name="count"> 要发送的数据报的字节数组的长度。 </param>
    /// <returns> 一个表示异步发送操作的任务。 </returns>
    Task SendAsync(byte[] data, int index, int count);
}
