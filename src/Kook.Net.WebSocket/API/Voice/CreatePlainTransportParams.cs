using System.Text.Json.Serialization;

namespace Kook.API.Voice;

internal class CreatePlainTransportParams
{
    [JsonPropertyName("comedia")]
    public required bool Comedia { get; set; }

    [JsonPropertyName("rtcpMux")]
    public required bool RtcpMultiplexing { get; set; }

    [JsonPropertyName("type")]
    public required string Type { get; set; }
}
