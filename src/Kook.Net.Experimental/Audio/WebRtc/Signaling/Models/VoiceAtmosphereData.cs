using System.Text.Json.Serialization;

namespace Kook.Audio.WebRtc.Signaling;

/// <summary>
///     <c>atmosphere</c> 通知的载荷。
/// </summary>
internal class VoiceAtmosphereData
{
    [JsonPropertyName("peerId")]
    public string? PeerId { get; set; }

    // 氛围道具的 ID。实测为整数（例如 11065），语义源码未展开
    [JsonPropertyName("itemId")]
    public ulong? ItemId { get; set; }
}
