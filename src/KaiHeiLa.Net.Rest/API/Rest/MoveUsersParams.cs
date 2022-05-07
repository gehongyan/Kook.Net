using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Rest;

internal class MoveUsersParams
{
    [JsonPropertyName("target_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong ChannelId { get; set; }
    
    [JsonPropertyName("user_ids")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong[] UserIds { get; set; }
}