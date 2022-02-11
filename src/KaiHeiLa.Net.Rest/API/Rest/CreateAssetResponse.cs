using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Rest;

internal class CreateAssetResponse
{
    [JsonPropertyName("url")]
    public string Url { get; set; }
}