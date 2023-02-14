using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class CreateAssetResponse
{
    [JsonPropertyName("url")]
    public string Url { get; set; }
}
