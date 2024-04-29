using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API;

internal class Intimacy
{
    [JsonPropertyName("img_url")]
    public required string ImageUrl { get; set; }

    [JsonPropertyName("social_info")]
    public required string SocialInfo { get; set; }

    [JsonPropertyName("last_read")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset LastReadAt { get; set; }

    [JsonPropertyName("last_modify")]
    [JsonConverter(typeof(NullableDateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset? LastModifyAt { get; set; }

    [JsonPropertyName("score")]
    public int Score { get; set; }

    [JsonPropertyName("img_list")]
    public required IntimacyImage[] Images { get; set; }
}

internal class IntimacyImage
{
    [JsonPropertyName("id")]
    public uint Id { get; set; }

    [JsonPropertyName("url")]
    public required string Url { get; set; }
}
