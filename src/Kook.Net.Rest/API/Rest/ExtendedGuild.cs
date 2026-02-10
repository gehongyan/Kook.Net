using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API.Rest;

internal class ExtendedGuild : Guild
{
    [JsonPropertyName("features")]
    [JsonConverter(typeof(GuildFeaturesConverter))]
    public required GuildFeatures Features { get; set; }

    [JsonPropertyName("boost_num")]
    public int BoostSubscriptionCount { get; set; }

    [JsonPropertyName("buffer_boost_num")]
    public int BufferBoostSubscriptionCount { get; set; }

    [JsonPropertyName("level")]
    public BoostLevel BoostLevel { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("auto_delete_time")]
    public string? AutoDeleteTime { get; set; }

    [JsonPropertyName("recommend_info")]
    public RecommendInfo? RecommendInfo { get; set; }

    [JsonPropertyName("user_config")]
    public UserConfig? UserConfig { get; set; }

    [JsonPropertyName("join_restrictions")]
    public JoinRestrictions? JoinRestrictions { get; set; }
}
