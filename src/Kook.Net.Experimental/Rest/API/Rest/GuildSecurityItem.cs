using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API.Rest;

internal class GuildSecurityItem
{
    [JsonPropertyName("action")]
    public required BehaviorRestrictionType Action { get; set; }

    [JsonPropertyName("conditions")]
    public required GuildSecurityCondition[] Conditions { get; set; }

    [JsonPropertyName("created_at")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public required DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("limit_time")]
    public required int LimitTime { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("switch")]
    public required bool Switch { get; set; }

    [JsonPropertyName("updated_at")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public required DateTimeOffset UpdatedAt { get; set; }
}
