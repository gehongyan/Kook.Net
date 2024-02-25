using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class CreateOrRemoveGuildMuteDeafParams
{
    [JsonPropertyName("guild_id")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public ulong GuildId { get; set; }

    [JsonPropertyName("target_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("type")]
    public MuteOrDeafType Type { get; set; }
}
