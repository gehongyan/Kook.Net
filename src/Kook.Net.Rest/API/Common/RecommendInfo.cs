using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API;

internal class RecommendInfo
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("open_id")]
    [JsonConverter(typeof(NullableUInt32Converter))]
    public uint? OpenId { get; set; }

    [JsonPropertyName("default_channel_id")]
    public ulong DefaultChannelId { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("icon")]
    public required string Icon { get; set; }

    [JsonPropertyName("banner")]
    public required string Banner { get; set; }

    [JsonPropertyName("desc")]
    public required string Description { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("tag")]
    public required string Tag { get; set; }

    [JsonPropertyName("features")]
    [JsonConverter(typeof(GuildFeaturesConverter))]
    public required GuildFeatures Features { get; set; }

    [JsonPropertyName("certifications")]
    public GuildCertification[]? Certifications { get; set; }

    [JsonPropertyName("level")]
    public BoostLevel BoostLevel { get; set; }

    [JsonPropertyName("custom_id")]
    public required string CustomId { get; set; }

    [JsonPropertyName("is_official_partner")]
    [JsonConverter(typeof(NumberBooleanConverter))]
    public bool IsOfficialPartner { get; set; }

    [JsonPropertyName("sort")]
    public int Sort { get; set; }

    [JsonPropertyName("audit_status")]
    public int AuditStatus { get; set; }

    [JsonPropertyName("update_day_gap")]
    public int UpdateDayInterval { get; set; }
}
