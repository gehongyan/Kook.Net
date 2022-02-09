using System.Text.Json.Serialization;

namespace KaiHeiLa.API;

internal class VideoModule : ModuleBase
{
    [JsonPropertyName("src")] public string Source { get; set; }
    [JsonPropertyName("title")] public string Title { get; set; }
}