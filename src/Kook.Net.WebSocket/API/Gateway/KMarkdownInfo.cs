using System.Text.Json.Serialization;

namespace Kook.API.Gateway;

internal class KMarkdownInfo
{
    [JsonPropertyName("raw_content")]
    public string RawContent { get; set; }
}