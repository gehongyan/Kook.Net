using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API.Gateway;

internal class GuildMemberUpdateEvent
{
    [JsonPropertyName("user_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong UserId { get; set; }
    
    [JsonPropertyName("nickname")]
    public string Nickname { get; set; }
}