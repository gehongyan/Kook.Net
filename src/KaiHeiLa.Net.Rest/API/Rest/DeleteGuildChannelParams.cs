using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Rest;

internal class DeleteGuildChannelParams
{
    [JsonPropertyName("channel_id")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString)]
    public ulong ChannelId { get; set; }

    public static implicit operator DeleteGuildChannelParams(ulong channelId) => new() {ChannelId = channelId};
}