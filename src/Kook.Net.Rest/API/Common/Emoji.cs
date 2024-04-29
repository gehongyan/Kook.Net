using System.Text.Json.Serialization;

namespace Kook.API;

internal class Emoji
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("emoji_type")]
    public EmojiType? Type { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("user_info")]
    public User? UploadedBy { get; set; }
}
