namespace Kook;

/// <summary>
///     表示一个通用的用户语音连接状态。
/// </summary>
public interface IVoiceState
{
    /// <summary>
    ///     获取此用户是否被服务器静音。
    /// </summary>
    /// <remarks>
    ///     被服务器静音表示无法在语音频道内接收来自其他用户的语音。
    /// </remarks>
    bool? IsDeafened { get; }

    /// <summary>
    ///     获取此用户是否被服务器闭麦。
    /// </summary>
    /// <remarks>
    ///     被服务器闭麦表示无法在语音频道内发言。
    /// </remarks>
    bool? IsMuted { get; }

    /// <summary>
    ///     获取此用户当前所连接的语音频道。
    /// </summary>
    IVoiceChannel? VoiceChannel { get; }

    /// <summary>
    ///     获取此用户连接的所有语音频道。
    /// </summary>
    IReadOnlyCollection<IVoiceChannel> VoiceChannels { get; }
}
