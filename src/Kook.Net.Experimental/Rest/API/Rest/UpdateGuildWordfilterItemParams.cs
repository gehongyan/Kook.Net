using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class UpdateGuildWordfilterItemParams
{
    [JsonPropertyName("guild_id")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public required ulong GuildId { get; set; }

    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("targets")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ContentFilterTarget? Targets { get; set; }

    [JsonPropertyName("handlers")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ContentFilterHandler[]? Handlers { get; set; }

    [JsonPropertyName("exemption_list")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ContentFilterExemption[]? Exemptions { get; set; }

    [JsonPropertyName("switch")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Switch { get; set; }
}
