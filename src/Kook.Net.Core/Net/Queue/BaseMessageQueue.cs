using System.Text.Json;

namespace Kook.Net.Queue;

/// <summary>
///     表示一个通用的消息队列抽象类。
/// </summary>
public abstract class BaseMessageQueue : IMessageQueue
{
    /// <summary>
    ///     初始化一个 <see cref="BaseMessageQueue"/> 类的新实例。
    /// </summary>
    /// <param name="eventHandler"> 用于处理消息的事件处理程序。 </param>
    protected BaseMessageQueue(Func<int, JsonElement, Task> eventHandler)
    {
        EventHandler = eventHandler;
    }

    /// <summary>
    ///     消息队列是否处理事件顺序。
    /// </summary>
    /// <remarks>
    ///     如果此队列实现维护事件的顺序，则应将此属性设置为 <c>true</c>，网关线程会在 <see cref="EventHandler"/>
    ///     调用时更新所收到的最新事件序号。否则，网关线程会在 <see cref="EnqueueAsync"/>
    ///     前就更新最新事件序号。
    /// </remarks>
    public virtual bool HandleSequence => false;

    /// <summary>
    ///     获取消息队列的事件处理程序。
    /// </summary>
    protected Func<int, JsonElement, Task> EventHandler { get; }

    /// <summary>
    ///     当消息队列请求网关重连时发生（如缓冲溢出、等待超时）。订阅方应在网关线程上调用重连逻辑（如 <c>Connection.Error(e.Exception)</c>）。
    /// </summary>
    public event EventHandler<ReconnectRequestedEventArgs>? ReconnectRequested;

    /// <summary>
    ///     引发 <see cref="ReconnectRequested"/> 事件。
    /// </summary>
    /// <param name="e"> 事件参数。 </param>
    protected virtual void OnReconnectRequested(ReconnectRequestedEventArgs e) => ReconnectRequested?.Invoke(this, e);

    /// <summary>
    ///     启动消息队列的处理。
    /// </summary>
    /// <param name="cancellationToken"> 用于取消该操作的取消令牌。 </param>
    /// <returns> 表示一个异步启动操作的任务。 </returns>
    public abstract Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     停止消息队列的处理。
    /// </summary>
    /// <param name="cancellationToken"> 用于取消该操作的取消令牌。 </param>
    /// <returns> 表示一个异步停止操作的任务。 </returns>
    public abstract Task StopAsync(CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task EnqueueAsync(JsonElement payload, int sequence, CancellationToken cancellationToken = default);
}
