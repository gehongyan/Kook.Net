using Kook.WebSocket;
using MassTransit;

namespace Kook.Net.Queue.MassTransit;

/// <summary>
///     Represents a MassTransit consumer for Kook message queue.
/// </summary>
internal class KookMessageQueueMassTransitConsumer : IConsumer<GatewayMessageWrapper>
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
    public async Task Consume(ConsumeContext<GatewayMessageWrapper> context)
    {
        await _client.ProcessGatewayEventAsync(context.Message.Payload).ConfigureAwait(false);
    }
}
