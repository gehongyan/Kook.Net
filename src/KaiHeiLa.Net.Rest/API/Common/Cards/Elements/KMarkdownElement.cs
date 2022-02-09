using System.Text.Json.Serialization;

namespace KaiHeiLa.API;

internal class KMarkdownElement : ElementBase
{
    [JsonPropertyName("content")]
    public string Content { get; set; }
}