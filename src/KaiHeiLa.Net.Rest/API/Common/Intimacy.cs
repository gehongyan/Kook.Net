using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API;

internal class Intimacy
{
    [JsonPropertyName("img_url")] public string ImageUrl { get; set; }
    [JsonPropertyName("social_info")] public string SocialInfo { get; set; }

    [JsonPropertyName("last_read")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset LastReadAt { get; set; }

    [JsonPropertyName("score")] public int Score { get; set; }
    [JsonPropertyName("img_list")] public IntimacyImage[] Images { get; set; }
}

internal class IntimacyImage
{
    [JsonPropertyName("id")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString)]
    public int Id { get; set; }

    [JsonPropertyName("url")] public string Url { get; set; }
}