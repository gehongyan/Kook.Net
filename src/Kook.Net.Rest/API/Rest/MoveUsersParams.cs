using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class MoveUsersParams
{
    [JsonPropertyName("target_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("user_ids")]
    public ulong[] UserIds { get; set; }
}
