using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class DisconnectUserParams
{
    [JsonPropertyName("channel_id")]
    public required ulong ChannelId { get; set; }

    [JsonPropertyName("user_id")]
    public required ulong UserId { get; set; }
}
