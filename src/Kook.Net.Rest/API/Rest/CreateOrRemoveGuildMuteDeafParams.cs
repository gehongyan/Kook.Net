using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class CreateOrRemoveGuildMuteDeafParams
{
    [JsonPropertyName("guild_id")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public required ulong GuildId { get; set; }

    [JsonPropertyName("target_id")]
    public required ulong UserId { get; set; }

    [JsonPropertyName("type")]
    public required MuteOrDeafType Type { get; set; }
}
