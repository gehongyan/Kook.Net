using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Rest;

internal class UpdateIntimacyValueParams
{
    [JsonPropertyName("user_id")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString)]
    public ulong UserId { get; set; }

    [JsonPropertyName("score")] public int Score { get; set; }

    [JsonPropertyName("social_info")] public string SocialInfo { get; set; }

    [JsonPropertyName("img_id")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString)]
    public uint ImageId { get; set; }
}