using System.Text.Json.Serialization;

namespace Kook.API.Gateway;

internal class LiveStatusChangeEvent
{
    [JsonPropertyName("channel")]
    public required Channel Channel { get; set; }

    [JsonPropertyName("user")]
    public required LiveStreamUser User { get; set; }
}
