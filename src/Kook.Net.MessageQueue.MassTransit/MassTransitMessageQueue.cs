using System.Text.Json;
using MassTransit;

namespace Kook.Net.Queue.MassTransit;

internal class MassTransitMessageQueue : BaseMessageQueue
{
    private readonly IBus _bus;

    internal MassTransitMessageQueue(Func<JsonElement, Task> eventHandler, IBus bus)
        : base(eventHandler)
    {
        _bus = bus;
    }

    /// <inheritdoc />
    public override async Task EnqueueAsync(JsonElement payload, int sequence, CancellationToken cancellationToken = default)
    {
        GatewayMessageWrapper wrapper = new()
        {
            Payload = payload,
            Sequence = sequence
        };
        await _bus.Publish(wrapper, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override Task StartAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    /// <inheritdoc />
    public override Task StopAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
