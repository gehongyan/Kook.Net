using System.Text.Json.Serialization;

namespace Kook.Audio.WebRtc.Signaling;

/// <summary>
///     <c>consumerClosed</c> 通知的载荷。
/// </summary>
internal class VoiceConsumerRefData
{
    [JsonPropertyName("consumerId")]
    public string? ConsumerId { get; set; }
}
