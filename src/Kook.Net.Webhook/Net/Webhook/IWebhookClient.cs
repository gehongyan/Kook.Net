namespace Kook.Net.Webhooks;

/// <summary>
///     表示一个通用的基于 Webhook 的网关客户端。
/// </summary>
public interface IWebhookClient : IDisposable
{
    /// <summary>
    ///     当接收到二进制消息时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:System.Byte"/>[] 参数是消息的二进制数据。 </item>
    ///     <item> <see cref="T:System.Int32"/> 参数是数据的起始索引。 </item>
    ///     <item> <see cref="T:System.Int32"/> 参数是数据的长度。 </item>
    ///     </list>
    ///     事件返回值：返回一个表示异步操作的任务，任务的结果是一个字符串，表示响应消息。
    /// </remarks>
    event Func<byte[], int, int, Task<string?>>? BinaryMessage;

    /// <summary>
    ///     当接收到文本消息时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:System.String"/> 参数是消息的文本。 </item>
    ///     </list>
    ///     事件返回值：返回一个表示异步操作的任务，任务的结果是一个字符串，表示响应消息。
    /// </remarks>
    event Func<string, Task<string?>>? TextMessage;

    /// <summary>
    ///     当连接关闭时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:System.Exception"/> 参数是引发关闭的异常。 </item>
    ///     </list>
    /// </remarks>
    event Func<Exception, Task>? Closed;

    /// <summary>
    ///     处理文本消息。
    /// </summary>
    /// <param name="requestBody"> 请求体。 </param>
    /// <returns> 一个表示异步操作的任务，任务的结果是一个字符串，表示响应消息。 </returns>
    Task<string?> HandleTextMessageAsync(string requestBody);

    /// <summary>
    ///     处理二进制消息。
    /// </summary>
    /// <param name="data"> 消息的二进制数据。 </param>
    /// <param name="index"> 数据的起始索引。 </param>
    /// <param name="count"> 数据的长度。 </param>
    /// <returns> 一个表示异步操作的任务，任务的结果是一个字符串，表示响应消息。 </returns>
    Task<string?> HandleBinaryMessageAsync(byte[] data, int index, int count);
}
