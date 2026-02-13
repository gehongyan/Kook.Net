using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API.Rest;

internal class GuildSecurityWordfilterItem
{
    [JsonPropertyName("created_at")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public required DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("exemption_list")]
    public required ContentFilterExemption[] Exemptions { get; set; }

    [JsonPropertyName("handlers")]
    public required ContentFilterHandler[] Handlers { get; set; }

    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("switch")]
    public required bool Switch { get; set; }

    [JsonPropertyName("targets")]
    public required ContentFilterTarget Targets { get; set; }

    [JsonPropertyName("type")]
    public required ContentFilterType Type { get; set; }

    [JsonPropertyName("updated_at")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public required DateTimeOffset UpdatedAt { get; set; }
}
