using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class DeleteGuildSecurityItemParams
{
    [JsonPropertyName("guild_id")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public required ulong GuildId { get; set; }

    [JsonPropertyName("id")]
    public required string Id { get; set; }
}
