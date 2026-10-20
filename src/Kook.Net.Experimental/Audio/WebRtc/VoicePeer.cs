using Kook.Audio.WebRtc.Signaling;

namespace Kook.Audio.WebRtc;

/// <summary>
///     表示一个在语音频道内的成员，是该成员加入语音频道时的信息快照。
/// </summary>
/// <remarks>
///     除 <see cref="Id"/> 外，所有属性都是可空的：服务端不保证每次都下发全部字段，<c>null</c> 表示服务端没有给出该字段。
///     成员之后的状态变化（例如麦克风静音）不会更新此快照，请通过 <see cref="Kook.Audio.WebRtc.IWebRtcAudioClient"/> 上对应的事件获知。
/// </remarks>
public sealed class VoicePeer
{
    /// <summary>
    ///     获取该成员的用户 ID。
    /// </summary>
    public ulong Id { get; }

    /// <summary>
    ///     获取该成员在服务器中的显示名称。
    /// </summary>
    public string? DisplayName { get; }

    /// <summary>
    ///     获取该成员的用户名。
    /// </summary>
    public string? Username { get; }

    /// <summary>
    ///     获取该成员的识别号。
    /// </summary>
    public string? IdentifyNumber { get; }

    /// <summary>
    ///     获取该成员的头像 URL。
    /// </summary>
    public string? Avatar { get; }

    /// <summary>
    ///     获取一个值，指示该成员是否是 VIP 用户。
    /// </summary>
    public bool? IsVip { get; }

    /// <summary>
    ///     获取该成员所使用的设备。
    /// </summary>
    /// <remarks>
    ///     实测出现过的取值有 <c>pc</c>、<c>web</c>、<c>android</c> 和 <c>bot</c>。服务端没有文档化此字段的取值范围，
    ///     因此这里保留原始字符串，而不是枚举。
    /// </remarks>
    public string? Device { get; }

    /// <summary>
    ///     获取一个值，指示该成员的麦克风是否处于静音状态。
    /// </summary>
    public bool? IsMicrophoneMuted { get; }

    /// <summary>
    ///     获取一个值，指示该成员的耳机是否处于静音状态。
    /// </summary>
    public bool? IsHeadsetMuted { get; }

    /// <summary>
    ///     获取一个值，指示该成员是否正在播放伴奏。
    /// </summary>
    public bool? IsPlayingAccompaniment { get; }

    /// <summary>
    ///     获取该成员的语音权限与服务端静音状态。
    /// </summary>
    public VoicePeerPermissions Permissions { get; }

    private VoicePeer(ulong id, VoicePeerInfo info)
    {
        Id = id;
        DisplayName = info.DisplayName;
        Username = info.Username;
        IdentifyNumber = info.IdentifyNumber;
        Avatar = info.Avatar;
        IsVip = info.IsVip;
        Device = info.Device;
        IsMicrophoneMuted = info.PausedProducer;
        IsHeadsetMuted = info.PausedHeadset;
        IsPlayingAccompaniment = info.Accompaniment;
        Permissions = new VoicePeerPermissions(info.IsServerMute, null, info.PermissionFreeMic, null,
            info.PermissionSay, info.PermissionVoiceManage);
    }

    /// <summary>
    ///     从信令下发的成员信息创建。成员的用户 ID 缺失或无法解析时返回 <c>null</c>。
    /// </summary>
    internal static VoicePeer? Create(VoicePeerInfo info) =>
        ulong.TryParse(info.PeerId ?? info.Id, out ulong id) ? new VoicePeer(id, info) : null;
}
