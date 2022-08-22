using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API;

internal class Intimacy
{
    [JsonPropertyName("img_url")] public string ImageUrl { get; set; }
    [JsonPropertyName("social_info")] public string SocialInfo { get; set; }

    [JsonPropertyName("last_read")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset LastReadAt { get; set; }
    
    [JsonPropertyName("last_modify")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset LastModifyAt { get; set; }

    [JsonPropertyName("score")] public int Score { get; set; }
    [JsonPropertyName("img_list")] public IntimacyImage[] Images { get; set; }
}

internal class IntimacyImage
{
    [JsonPropertyName("id")]
    public uint Id { get; set; }

    [JsonPropertyName("url")] public string Url { get; set; }
}