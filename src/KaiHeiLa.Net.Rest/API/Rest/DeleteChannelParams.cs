using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Rest;

internal class DeleteChannelParams
{
    [JsonPropertyName("channel_id")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString)]
    public ulong ChannelId { get; set; }

    public static implicit operator DeleteChannelParams(ulong channelId) => new() {ChannelId = channelId};
}