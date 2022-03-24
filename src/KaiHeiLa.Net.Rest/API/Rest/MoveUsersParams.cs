using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Rest;

internal class MoveUsersParams
{
    [JsonPropertyName("target_id")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString)]
    public ulong ChannelId { get; set; }
    
    [JsonPropertyName("user_ids")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString)]
    public ulong[] UserIds { get; set; }
}