using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class CreateGuildRoleParams
{
    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Name { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }
}