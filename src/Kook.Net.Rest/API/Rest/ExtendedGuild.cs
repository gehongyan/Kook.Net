using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class ExtendedGuild : Guild
{
    [JsonPropertyName("features")]
    public object[] Features { get; set; }

    [JsonPropertyName("boost_num")]
    public int BoostSubscriptionCount { get; set; }

    [JsonPropertyName("buffer_boost_num")]
    public int BufferBoostSubscriptionCount { get; set; }

    [JsonPropertyName("level")]
    public BoostLevel BoostLevel { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("auto_delete_time")]
    public string AutoDeleteTime { get; set; }

    [JsonPropertyName("recommend_info")]
    public RecommendInfo RecommendInfo { get; set; }
}
