namespace Kook.Audio;

/// <summary>
///     表示语音频道中其它语音客户端的权限信息。
/// </summary>
public readonly struct PeerPermissionInfo
{
    /// <summary>
    ///     获取是否被服务器闭麦。
    /// </summary>
    public bool MutedByGuild { get; init; }

    /// <summary>
    ///     获取是否被服务器静音。
    /// </summary>
    public bool DeafenedByGuild { get; init; }

    /// <summary>
    ///     获取是否可以在当前语音频道使用语音活性检测，以自由麦的方式不按键即说话。
    /// </summary>
    public bool CanUseVoiceActivity { get; init; }

    /// <summary>
    ///     获取是否可以连接到当前语音频道。
    /// </summary>
    public bool CanConnect { get; init; }

    /// <summary>
    ///     获取是否可以在当前语音频道中说话。
    /// </summary>
    public bool CanSpeak { get; init; }

    /// <summary>
    ///     获取是否可以管理当前语音频道。
    /// </summary>
    public bool CanManageVoice { get; init; }
}

