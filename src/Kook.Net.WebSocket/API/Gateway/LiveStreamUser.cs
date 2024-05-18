using System.Text.Json.Serialization;

namespace Kook.API.Gateway;

internal class LiveStreamUser : User
{
    [JsonPropertyName("live_info")]
    public required LiveInfo LiveStreamStatus { get; set; }
}

