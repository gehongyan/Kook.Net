using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class CreateAssetResponse
{
    [JsonPropertyName("url")]
    public required string Url { get; set; }
}
