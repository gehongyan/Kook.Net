using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class KeepVoiceGatewayAliveParams
{
    [JsonPropertyName("channel_id")]
    public required ulong ChannelId { get; set; }

    public static implicit operator KeepVoiceGatewayAliveParams(ulong channelId) => new() { ChannelId = channelId };
}
