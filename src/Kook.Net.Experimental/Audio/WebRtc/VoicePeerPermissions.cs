namespace Kook.Audio.WebRtc;

/// <summary>
///     表示一个成员在语音频道内的语音权限与服务端静音状态。
/// </summary>
/// <remarks>
///     所有属性都是可空的：服务端不保证每次都下发全部字段，<c>null</c> 表示服务端没有给出该字段，而不是 <c>false</c>。
/// </remarks>
public sealed class VoicePeerPermissions
{
    /// <summary>
    ///     获取一个值，指示该成员是否被服务端禁麦（管理员禁止其发言）。
    /// </summary>
    public bool? IsServerMuted { get; }

    /// <summary>
    ///     获取一个值，指示该成员是否被服务端禁听（管理员禁止其收听）。
    /// </summary>
    /// <remarks>
    ///     此字段只会出现在 <see cref="Kook.Audio.WebRtc.IWebRtcAudioClient.PeerPermissionChanged"/> 事件中，
    ///     成员加入时下发的成员信息里没有这个字段。
    /// </remarks>
    public bool? IsServerHeadsetMuted { get; }

    /// <summary>
    ///     获取一个值，指示该成员是否拥有自由发言的权限，即不需要按键说话。
    /// </summary>
    public bool? CanFreeMic { get; }

    /// <summary>
    ///     获取一个值，指示该成员是否拥有连接语音的权限。
    /// </summary>
    /// <remarks>
    ///     此字段的含义是根据名称推断的，尚未验证。它只会出现在
    ///     <see cref="Kook.Audio.WebRtc.IWebRtcAudioClient.PeerPermissionChanged"/> 事件中。
    /// </remarks>
    public bool? CanLink { get; }

    /// <summary>
    ///     获取一个值，指示该成员是否拥有发言的权限。
    /// </summary>
    public bool? CanSpeak { get; }

    /// <summary>
    ///     获取一个值，指示该成员是否拥有管理语音频道的权限。
    /// </summary>
    public bool? CanManageVoice { get; }

    internal VoicePeerPermissions(bool? isServerMuted, bool? isServerHeadsetMuted, bool? canFreeMic, bool? canLink,
        bool? canSpeak, bool? canManageVoice)
    {
        IsServerMuted = isServerMuted;
        IsServerHeadsetMuted = isServerHeadsetMuted;
        CanFreeMic = canFreeMic;
        CanLink = canLink;
        CanSpeak = canSpeak;
        CanManageVoice = canManageVoice;
    }
}
