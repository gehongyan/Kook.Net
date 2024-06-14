using System.Text.Json;

namespace Kook.Net.Queue.MassTransit;

internal record GatewayMessageWrapper
{
    public JsonElement Payload { get; set; }

    public int Sequence { get; set; }
}
