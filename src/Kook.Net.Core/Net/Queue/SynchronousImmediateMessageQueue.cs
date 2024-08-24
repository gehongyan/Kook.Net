using System.Text.Json;

namespace Kook.Net.Queue.SynchronousImmediate;

/// <summary>
///     表示一个同步处理消息队列。
/// </summary>
/// <remarks>
///     此消息队列会在接收到网关事件后调用
///     <see cref="M:Kook.Net.Queue.SynchronousImmediate.SynchronousImmediateMessageQueue.EnqueueAsync(System.Text.Json.JsonElement,System.Int32,System.Threading.CancellationToken)"/>
///     时立即使用构造函数中传入的 <c>eventHandler</c> 委托同步进行处理，处理完成后，该方法才会返回。
/// </remarks>
public class SynchronousImmediateMessageQueue : BaseMessageQueue
{
    /// <inheritdoc />
    public SynchronousImmediateMessageQueue(Func<JsonElement, Task> eventHandler)
        : base(eventHandler)
    {
    }

    /// <inheritdoc />
    public override async Task EnqueueAsync(JsonElement payload, int sequence,
        CancellationToken cancellationToken = default)
    {
        await EventHandler(payload).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override Task StartAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    /// <inheritdoc />
    public override Task StopAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
