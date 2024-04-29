using System.Text.Json.Serialization;

namespace Kook.API.Voice;

internal class CreatePlainTransportResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("ip")]
    public required string Ip { get; set; }

    [JsonPropertyName("port")]
    public int Port { get; set; }

    [JsonPropertyName("rtcpPort")]
    public int RtcpPort { get; set; }
}
