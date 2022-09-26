using System.Text.Json.Serialization;

namespace Kook.API.Gateway;

internal class GuildEmojiEvent
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("emoji_type")]
    public EmojiType Type { get; set; }
}