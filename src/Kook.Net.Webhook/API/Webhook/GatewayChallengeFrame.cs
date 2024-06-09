using System.Text.Json.Serialization;

namespace Kook.API.Webhook;

internal class GatewayChallengeFrame
{
    [JsonPropertyName("challenge")]
    public required string Challenge { get; set; }
}
