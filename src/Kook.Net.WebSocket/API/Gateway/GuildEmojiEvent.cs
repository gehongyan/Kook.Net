using System.Text.Json.Serialization;

namespace Kook.API.Gateway;

internal class GuildEmojiEvent
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("emoji_type")]
    public EmojiType Type { get; set; }
}
