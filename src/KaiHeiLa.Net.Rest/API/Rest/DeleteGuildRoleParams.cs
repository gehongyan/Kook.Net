using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Rest;

internal class DeleteGuildRoleParams
{
    [JsonPropertyName("guild_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong GuildId { get; set; }

    [JsonPropertyName("role_id")] public uint Id { get; set; }
}