using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class DisconnectUserParams
{
    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }
}
