using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class UpdateIntimacyValueParams
{
    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("score")]
    public int? Score { get; set; }

    [JsonPropertyName("social_info")]
    public string? SocialInfo { get; set; }

    [JsonPropertyName("img_id")]
    public uint? ImageId { get; set; }
}
