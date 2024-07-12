using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class DisposeVoiceGatewayParams
{
    [JsonPropertyName("channel_id")]
    public required ulong ChannelId { get; set; }

    public static implicit operator DisposeVoiceGatewayParams(ulong channelId) => new() { ChannelId = channelId };
}
