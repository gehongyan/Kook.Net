using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class CreateGuildSecurityItemParams
{
    [JsonPropertyName("guild_id")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public required ulong GuildId { get; set; }

    [JsonPropertyName("action")]
    public required GuildBehaviorRestrictionType Action { get; set; }

    [JsonPropertyName("conditions")]
    public required GuildSecurityCondition[] Conditions { get; set; }

    [JsonPropertyName("limit_time")]
    public required int LimitTime { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("switch")]
    public required bool Switch { get; set; }
}
