using Kook.WebSocket;
using MassTransit;

namespace Kook.Net.Queue.MassTransit;

internal class KookMessageQueueMassTransitConsumer : IConsumer<GatewayMessageWrapper>
{
    private readonly KookSocketClient _client;

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
