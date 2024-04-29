using System.Text.Json.Serialization;

namespace Kook.API;

internal class KMarkdownElement : ElementBase
{
    [JsonPropertyName("content")]
    public required string Content { get; set; }
}
