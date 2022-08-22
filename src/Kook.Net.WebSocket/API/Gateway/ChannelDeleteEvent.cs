using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API.Gateway;

internal class ChannelDeleteEvent
{
    [JsonPropertyName("id")]
    public ulong ChannelId { get; set; }
    
    [JsonPropertyName("deleted_at")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset DeletedAt { get; set; }
}