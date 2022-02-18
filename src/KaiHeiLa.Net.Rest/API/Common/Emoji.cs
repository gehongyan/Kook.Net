using System.Text.Json.Serialization;

namespace KaiHeiLa.API;

internal class Emoji
{
    [JsonPropertyName("emoji_type")]
    public EmojiType Type { get; set; }
    
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("user_info")]
    public User UploadedBy { get; set; }
}