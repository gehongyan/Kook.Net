using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Gateway;

internal class KMarkdownInfo
{
    [JsonPropertyName("raw_content")]
    public string RawContent { get; set; }
}