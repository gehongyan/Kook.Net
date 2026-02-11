using System.Threading.Channels;
using System.Diagnostics;
using System.Text.Json;

namespace Kook.Net.Queue.InMemory;

internal class InMemoryMessageQueue : BaseMessageQueue
{
    private readonly Channel<QueueItem> _channel;
    private CancellationTokenSource? _cancellationTokenSource;

    internal InMemoryMessageQueue(Func<int, JsonElement, Task> eventHandler)
        : base(eventHandler)
    {
        _channel = Channel.CreateUnbounded<QueueItem>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = true,
        });
    }

    /// <inheritdoc />
    public override Task StartAsync(CancellationToken cancellationToken = default)
    {
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        Task.Run(async () =>
        {
            await foreach (QueueItem gatewayEvent in _channel.Reader.ReadAllAsync(_cancellationTokenSource.Token))
            {
                try
                {
                    await EventHandler(gatewayEvent.Sequence, gatewayEvent.Payload).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error processing message: {ex}");
                }
            }
        }, _cancellationTokenSource.Token);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task StopAsync(CancellationToken cancellationToken = default)
    {
        _channel.Writer.Complete();
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override async Task EnqueueAsync(JsonElement payload, int sequence,
        CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(new QueueItem(sequence, payload), cancellationToken).ConfigureAwait(false);
    }
}
