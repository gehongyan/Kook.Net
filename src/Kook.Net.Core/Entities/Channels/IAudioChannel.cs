using Kook.Audio;

namespace Kook;

/// <summary>
///     表示一个通用音频频道。
/// </summary>
public interface IAudioChannel : IChannel
{
    /// <summary>
    ///     获取此音频频道的语音区域设置是否覆写了服务器的语音区域设置。
    /// </summary>
    bool? IsVoiceRegionOverwritten { get; }

    /// <summary>
    ///     获取此音频频道所设置的语音服务器区域。
    /// </summary>
    /// <remarks>
    ///     <note type="warning">
    ///         对于语音频道区域设置功能发布之前创建的语音频道，该属性可能为空。
    ///     </note>
    /// </remarks>
    /// <seealso cref="P:Kook.IGuild.Region"/>
    string? VoiceRegion { get; }

    /// <summary>
    ///     获取语音客户端连接到此语音频道的语音服务器 URL。
    /// </summary>
    string? ServerUrl { get; }

    // /// <param name="selfDeaf"> 指定连接到此语音客户端时是否应该对自身静音。 </param>
    // /// <param name="selfMute"> 指定连接到此语音客户端时是否应该对自身闭麦。 </param>
    /// <summary>
    ///     连接到此音频频道。
    /// </summary>
    /// <param name="external"> 指定语音客户端是否是由外部管理的。当设置为 <see langword="true"/> 时，当前方法不会尝试连接到语音频道。 </param>
    /// <param name="disconnect"> 指定语音客户端在连接到新的语音频道之前是否应调用断开连接。 </param>
    /// <param name="password"> 指定客户端连接到设置了密码的语音频道时所使用的密码。 </param>
    /// <returns>
    ///     一个表示音频连接操作的异步任务。任务的结果是一个负责音频连接的 <see cref="IAudioClient"/> 实例；如果
    ///     <paramref name="external"/> 为 <see langword="true"/>，则会返回 <see langword="null"/>。
    /// </returns>
    Task<IAudioClient?> ConnectAsync( /*bool selfDeaf = false, bool selfMute = false, */
        bool external = false, bool disconnect = true, string? password = null);

    /// <summary>
    ///     断开与此音频频道的连接。
    /// </summary>
    /// <returns> 一个表示音频断开连接操作的异步任务。 </returns>
    Task DisconnectAsync();
}
