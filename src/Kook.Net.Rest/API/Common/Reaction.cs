using System.Text.Json.Serialization;

namespace Kook.API;

internal class Reaction
{
    [JsonPropertyName("emoji")]
    public required Emoji Emoji { get; set; }

    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("me")]
    public bool IsMe { get; set; }
}
