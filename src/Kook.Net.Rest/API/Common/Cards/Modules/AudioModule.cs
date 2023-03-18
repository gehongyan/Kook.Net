using System.Text.Json.Serialization;

namespace Kook.API;

internal class AudioModule : ModuleBase
{
    [JsonPropertyName("src")]
    public string Source { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("cover")]
    public string Cover { get; set; }
}
