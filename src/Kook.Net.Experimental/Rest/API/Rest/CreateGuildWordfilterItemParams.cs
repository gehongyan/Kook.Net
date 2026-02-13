using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class CreateGuildWordfilterItemParams
{
    [JsonPropertyName("guild_id")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public required ulong GuildId { get; set; }

    [JsonPropertyName("type")]
    public required ContentFilterType Type { get; set; }

    [JsonPropertyName("targets")]
    public required ContentFilterTarget Targets { get; set; }

    [JsonPropertyName("handlers")]
    public required ContentFilterHandler[] Handlers { get; set; }

    [JsonPropertyName("exemption_list")]
    public required ContentFilterExemption[] Exemptions { get; set; }

    [JsonPropertyName("switch")]
    public required bool Switch { get; set; }
}
