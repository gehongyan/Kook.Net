using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Voice;

internal class CreatePlainTransportParams
{
    [JsonPropertyName("comedia")]
    public bool Comedia { get; set; }
    
    [JsonPropertyName("rtcpMux")]
    public bool RTCPMultiplexing { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }
}