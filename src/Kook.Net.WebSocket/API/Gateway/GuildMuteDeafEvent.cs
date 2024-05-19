using System.Text.Json.Serialization;
using Kook.API.Rest;
using Kook.Net.Converters;

namespace Kook.API.Gateway;

internal class GuildMuteDeafEvent
{
    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("type")]
    [JsonConverter(typeof(MuteOrDeafTypeConverter))]
    public MuteOrDeafType Type { get; set; }
}
