using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Rest;

internal class DeleteGuildChannelParams
{
    [JsonPropertyName("channel_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong ChannelId { get; set; }

    public static implicit operator DeleteGuildChannelParams(ulong channelId) => new() {ChannelId = channelId};
}