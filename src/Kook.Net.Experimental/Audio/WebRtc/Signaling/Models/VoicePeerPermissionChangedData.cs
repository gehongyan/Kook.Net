using System.Text.Json.Serialization;

namespace Kook.Audio.WebRtc.Signaling;

/// <summary>
///     <c>peerPermissionChanged</c> 通知的载荷。对象可以是任意成员，包括 Bot 自己。
/// </summary>
internal class VoicePeerPermissionChangedData
{
    [JsonPropertyName("peerId")]
    public string? PeerId { get; set; }

    [JsonPropertyName("roomId")]
    public string? RoomId { get; set; }

    [JsonPropertyName("isServerMute")]
    public bool? IsServerMute { get; set; }

    [JsonPropertyName("isServerMuteHeadset")]
    public bool? IsServerMuteHeadset { get; set; }

    [JsonPropertyName("permissionFreeMic")]
    public bool? PermissionFreeMic { get; set; }

    // 含义推断为"连接语音"，未验证
    [JsonPropertyName("permissionLink")]
    public bool? PermissionLink { get; set; }

    [JsonPropertyName("permissionSay")]
    public bool? PermissionSay { get; set; }

    [JsonPropertyName("permissionVoiceManage")]
    public bool? PermissionVoiceManage { get; set; }
}
