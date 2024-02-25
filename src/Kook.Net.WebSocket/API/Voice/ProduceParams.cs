using System.Text.Json.Serialization;

namespace Kook.API.Voice;

internal class ProduceParams
{
    [JsonPropertyName("appData")]
    public object AppData { get; set; }

    [JsonPropertyName("kind")]
    public string Kind { get; set; }

    [JsonPropertyName("peerId")]
    public string PeerId { get; set; }

    [JsonPropertyName("rtpParameters")]
    public RtpParameters RtpParameters { get; set; }

    [JsonPropertyName("transportId")]
    public Guid TransportId { get; set; }
}

internal class RtpParameters
{
    [JsonPropertyName("codecs")]
    public CodecParams[] Codecs { get; set; }

    [JsonPropertyName("encodings")]
    public EncodingParams[] Encodings { get; set; }
}

internal class EncodingParams
{
    [JsonPropertyName("ssrc")]
    public uint Ssrc { get; set; }
}

internal class CodecParams
{
    [JsonPropertyName("channels")]
    public int Channels { get; set; }

    [JsonPropertyName("clockRate")]
    public int ClockRate { get; set; }

    [JsonPropertyName("mimeType")]
    public string MimeType { get; set; }

    [JsonPropertyName("parameters")]
    public Parameters Parameters { get; set; }

    [JsonPropertyName("payloadType")]
    public int PayloadType { get; set; }
}

internal class Parameters
{
    [JsonPropertyName("sprop-stereo")]
    public int SenderProduceStereo { get; set; }
}
