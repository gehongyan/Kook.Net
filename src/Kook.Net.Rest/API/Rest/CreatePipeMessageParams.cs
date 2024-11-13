using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class CreatePipeMessageParams
{
    [JsonPropertyName("content")]
    public required string Content { get; set; }
}
