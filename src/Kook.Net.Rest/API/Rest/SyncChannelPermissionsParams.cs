using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class SyncChannelPermissionsParams
{
    [JsonPropertyName("channel_id")]
    public required ulong ChannelId { get; set; }
}
