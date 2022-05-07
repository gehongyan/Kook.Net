using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Rest;

internal class AddOrRemoveRoleParams
{
    [JsonPropertyName("guild_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong GuildId { get; set; }

    [JsonPropertyName("user_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong UserId { get; set; }

    [JsonPropertyName("role_id")] 
    public uint RoleId { get; set; }
}