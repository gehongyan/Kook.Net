using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Audio.WebRtc.Signaling;

/// <summary>
///     成员信息，同时是 <c>initSpeaking</c> 响应中的 <c>peer</c> / <c>peers[]</c>、
///     <c>newPeer</c> 通知以及 <c>newConsumer</c> 的 <c>appData.peerInfo</c> 的载荷。
/// </summary>
internal class VoicePeerInfo
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("peerId")]
    public string? PeerId { get; set; }

    [JsonPropertyName("roomId")]
    public string? RoomId { get; set; }

    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("identify_num")]
    public string? IdentifyNumber { get; set; }

    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }

    [JsonPropertyName("vip_avatar")]
    public string? VipAvatar { get; set; }

    [JsonPropertyName("is_vip")]
    public bool? IsVip { get; set; }

    // 实测取值：pc、web、android、bot
    [JsonPropertyName("device")]
    public string? Device { get; set; }

    [JsonPropertyName("pausedProducer")]
    public bool? PausedProducer { get; set; }

    [JsonPropertyName("pausedHeadset")]
    public bool? PausedHeadset { get; set; }

    [JsonPropertyName("accompaniment")]
    public bool? Accompaniment { get; set; }

    [JsonPropertyName("permissionVoiceManage")]
    public bool? PermissionVoiceManage { get; set; }

    [JsonPropertyName("permissionFreeMic")]
    public bool? PermissionFreeMic { get; set; }

    [JsonPropertyName("permissionSay")]
    public bool? PermissionSay { get; set; }

    [JsonPropertyName("roles")]
    public IReadOnlyList<ulong>? Roles { get; set; }

    [JsonPropertyName("isServerMute")]
    public bool? IsServerMute { get; set; }

    [JsonPropertyName("extra")]
    public JsonElement? Extra { get; set; }

    [JsonExtensionData]
    public IDictionary<string, JsonElement>? ExtraData { get; set; }
}
