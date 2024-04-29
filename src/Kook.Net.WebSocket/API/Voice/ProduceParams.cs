using System.Text.Json.Serialization;

namespace Kook.API.Voice;

internal class ProduceParams
{
    [JsonPropertyName("appData")]
    public required object AppData { get; set; }

    [JsonPropertyName("kind")]
    public required string Kind { get; set; }

    [JsonPropertyName("peerId")]
    public required string PeerId { get; set; }

    [JsonPropertyName("rtpParameters")]
    public required RtpParameters RtpParameters { get; set; }

    [JsonPropertyName("transportId")]
    public required Guid TransportId { get; set; }
}

internal class RtpParameters
{
    [JsonPropertyName("codecs")]
    public required CodecParams[] Codecs { get; set; }

    [JsonPropertyName("encodings")]
    public required EncodingParams[] Encodings { get; set; }
}

internal class EncodingParams
{
    [JsonPropertyName("ssrc")]
    public required uint Ssrc { get; set; }
}

internal class CodecParams
{
    [JsonPropertyName("channels")]
    public required int Channels { get; set; }

    [JsonPropertyName("clockRate")]
    public required int ClockRate { get; set; }

    [JsonPropertyName("mimeType")]
    public required string MimeType { get; set; }

    [JsonPropertyName("parameters")]
    public required Parameters Parameters { get; set; }

    [JsonPropertyName("payloadType")]
    public required int PayloadType { get; set; }
}

internal class Parameters
{
    [JsonPropertyName("sprop-stereo")]
    public required int SenderProduceStereo { get; set; }
}
