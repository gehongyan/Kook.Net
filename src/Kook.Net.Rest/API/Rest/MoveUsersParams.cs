using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class MoveUsersParams
{
    [JsonPropertyName("target_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("user_ids")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public ulong[] UserIds { get; set; }
}
