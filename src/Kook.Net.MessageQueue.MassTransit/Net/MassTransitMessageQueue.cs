using System.Text.Json;
using Kook.WebSocket;
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
        string message = JsonSerializer.Serialize(payload);
        await _bus.Publish(message, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override Task StartAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    /// <inheritdoc />
    public override Task StopAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}

/// <summary>
///     Represents a MassTransit consumer for Kook message queue.
/// </summary>
public class KookMessageQueueMassTransitConsumer : IConsumer<string>
{
    private readonly KookSocketClient _client;

    /// <summary>
    ///     Initializes a new instance of the <see cref="KookMessageQueueMassTransitConsumer"/> class.
    /// </summary>
    /// <param name="client"> The Kook socket client. </param>
    public KookMessageQueueMassTransitConsumer(KookSocketClient client)
    {
        _client = client;
    }

    /// <inheritdoc />
    public async Task Consume(ConsumeContext<string> context)
    {
        JsonElement jsonElement = JsonSerializer.Deserialize<JsonElement>(context.Message);
        await _client.ProcessGatewayEventAsync(jsonElement).ConfigureAwait(false);
    }
}
