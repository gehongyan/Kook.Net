using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Gateway;

internal class Reaction
{
    [JsonPropertyName("channel_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("msg_id")] public Guid MessageId { get; set; }

    [JsonPropertyName("user_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public uint UserId { get; set; }

    [JsonPropertyName("emoji")] public Emoji Emoji { get; set; }
}