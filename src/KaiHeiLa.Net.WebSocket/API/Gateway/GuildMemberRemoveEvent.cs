using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API.Gateway;

internal class GuildMemberRemoveEvent
{
    [JsonPropertyName("user_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong UserId { get; set; }
    
    [JsonPropertyName("exited_at")]
    [JsonConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset ExitedAt { get; set; }
}