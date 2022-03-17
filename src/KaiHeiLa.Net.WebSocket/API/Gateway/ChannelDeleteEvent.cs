using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API.Gateway;

internal class ChannelDeleteEvent
{
    [JsonPropertyName("id")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString)]
    public ulong ChannelId { get; set; }
    
    [JsonPropertyName("deleted_at")]
    [JsonConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset DeletedAt { get; set; }
}