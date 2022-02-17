using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Rest;

internal class AddOrRemoveRoleParams
{
    [JsonPropertyName("guild_id")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString)]
    public ulong GuildId { get; set; }

    [JsonPropertyName("user_id")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString)]
    public ulong UserId { get; set; }

    [JsonPropertyName("role_id")] public uint RoleId { get; set; }
}