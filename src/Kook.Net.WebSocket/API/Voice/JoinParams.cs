using System.Text.Json.Serialization;

namespace Kook.API.Voice;

internal class JoinParams
{
    [JsonPropertyName("displayName")]
    public required string DisplayName { get; set; }
}
