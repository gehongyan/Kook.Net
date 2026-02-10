using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class GuildSecurityItemUpdateParams
{
    [JsonPropertyName("guild_id")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public required ulong GuildId { get; set; }

    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("action")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GuildBehaviorRestrictionType? Action { get; set; }

    [JsonPropertyName("conditions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GuildSecurityCondition[]? Conditions { get; set; }

    [JsonPropertyName("limit_time")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? LimitTime { get; set; }

    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; set; }

    [JsonPropertyName("switch")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Switch { get; set; }
}
