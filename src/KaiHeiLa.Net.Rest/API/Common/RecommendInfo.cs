using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API;

internal class RecommendInfo
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("guild_id")] public int GuildId { get; set; }
    [JsonPropertyName("type")] public int Type { get; set; }
    [JsonPropertyName("status")] public int Status { get; set; }
    [JsonPropertyName("banner")] public string Banner { get; set; }
    [JsonPropertyName("desc")] public string Desc { get; set; }
    [JsonPropertyName("tag_id")] public int TagID { get; set; }

    [JsonPropertyName("created_at")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeSecondsConverter))]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("updated_at")] 
    [JsonConverter(typeof(DateTimeOffsetUnixTimeSecondsConverter))]
    public DateTimeOffset UpdatedAt { get; set; }
    
    [JsonPropertyName("recommend_at")] 
    public int RecommendAt { get; set; }

    [JsonPropertyName("is_official_partner")]
    public int IsOfficialPartner { get; set; }

    [JsonPropertyName("sort")] 
    public int Sort { get; set; }
}