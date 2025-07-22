using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class RemoveGuildBanParams
{
    [JsonPropertyName("guild_id")]
    public required ulong GuildId { get; set; }

    [JsonPropertyName("target_id")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public required ulong UserId { get; set; }
}
