using System.Text.Json;

namespace Kook.Net.Queue;

/// <summary>
///     表示一个消息队列，用于从 KOOK 网关接收并处理事件。
/// </summary>
public interface IMessageQueue
{
    /// <summary>
    ///     将网关消息添加到队列中。
    /// </summary>
    /// <param name="payload"> 网关消息的有效负载。 </param>
    /// <param name="sequence"> 网关消息的序号。 </param>
    /// <param name="cancellationToken"> 用于取消该操作的取消令牌。 </param>
    /// <returns> 表示一个异步入队操作的任务。 </returns>
    Task EnqueueAsync(JsonElement payload, int sequence, CancellationToken cancellationToken = default);
}
