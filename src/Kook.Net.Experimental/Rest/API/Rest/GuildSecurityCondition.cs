using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class GuildSecurityCondition
{
    [JsonPropertyName("type")]
    public required string Type { get; set; }

    [JsonPropertyName("duration")]
    public int? Duration { get; set; }
}
