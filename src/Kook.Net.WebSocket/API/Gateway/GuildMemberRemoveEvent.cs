using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API.Gateway;

internal class GuildMemberRemoveEvent
{
    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }
    
    [JsonPropertyName("exited_at")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset ExitedAt { get; set; }
}