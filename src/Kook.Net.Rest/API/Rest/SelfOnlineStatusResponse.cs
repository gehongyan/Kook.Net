using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class SelfOnlineStatusResponse
{
    [JsonPropertyName("online")]
    public bool IsOnline { get; set; }

    [JsonPropertyName("online_os")]
    public required string[] OnlineOperatingSystems { get; set; }
}
