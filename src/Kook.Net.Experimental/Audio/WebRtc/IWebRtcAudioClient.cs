namespace Kook.Audio.WebRtc;

/// <summary>
///     表示一个基于 WebRTC 的实验性音频客户端。
/// </summary>
/// <remarks>
///     <para>
///         此客户端通过 KOOK 非官方文档化的 WebRTC 语音通道（protoo 信令加 mediasoup）收发音频，
///         与 <see cref="Kook.WebSocket.SocketVoiceChannel.ConnectAsync(System.Boolean,System.Boolean,System.String)"/>
///         所使用的基于 UDP 的官方语音接口是两套不同的机制。该机制随时可能变化。
///     </para>
///     <para>
///         客户端连接成功后默认处于静音状态，需要调用 <see cref="SetMicrophoneMutedAsync(System.Boolean)"/>
///         取消静音后，通过 <see cref="Kook.Audio.IAudioClient.CreatePcmStream(Kook.Audio.AudioApplication,System.Nullable{System.Int32},System.Int32,System.Int32)"/>
///         等方法所创建的音频流写入的音频才会被发送。
///     </para>
/// </remarks>
public interface IWebRtcAudioClient : IAudioClient
{
    /// <summary>
    ///     当有成员加入此语音频道时引发。
    /// </summary>
    /// <remarks>
    ///     客户端连接时已经在频道内的成员不会引发此事件。
    ///     <br />
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="Kook.Audio.WebRtc.VoicePeer"/> 参数是加入频道的成员的信息快照。 </item>
    ///     </list>
    /// </remarks>
    event Func<VoicePeer, Task> PeerJoined;

    /// <summary>
    ///     当有成员离开此语音频道时引发。
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         成员主动离开与被管理员移出语音频道，服务端下发的通知完全相同，无法区分。
    ///     </note>
    ///     <br />
    ///     如果该成员有音频输入流，<see cref="Kook.Audio.IAudioClient.StreamDestroyed"/> 事件会先于此事件引发。
    ///     <br />
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="System.UInt64"/> 参数是离开频道的成员的用户 ID。 </item>
    ///     </list>
    /// </remarks>
    event Func<ulong, Task> PeerLeft;

    /// <summary>
    ///     当有成员的麦克风静音状态变化时引发。
    /// </summary>
    /// <remarks>
    ///     <note type="warning">
    ///         服务端会把这类状态通知攒成大约每 2 秒一批下发，所以事件会有最长约 2 秒的延迟；
    ///         同时，某个成员的状态通知可能晚于他离开频道的通知到达，此时事件仍然会引发。
    ///         这两点是根据实测推断的，服务端并未在文档中说明。
    ///     </note>
    ///     <br />
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="System.UInt64"/> 参数是该成员的用户 ID。 </item>
    ///     <item> <see cref="System.Boolean"/> 参数指示该成员的麦克风是否已静音。 </item>
    ///     </list>
    /// </remarks>
    event Func<ulong, bool, Task> PeerMicrophoneMuteChanged;

    /// <summary>
    ///     当有成员的耳机静音状态变化时引发。
    /// </summary>
    /// <remarks>
    ///     与 <see cref="PeerMicrophoneMuteChanged"/> 一样，此事件有最长约 2 秒的延迟，并且可能晚于该成员离开频道的通知。
    ///     <br />
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="System.UInt64"/> 参数是该成员的用户 ID。 </item>
    ///     <item> <see cref="System.Boolean"/> 参数指示该成员的耳机是否已静音。 </item>
    ///     </list>
    /// </remarks>
    event Func<ulong, bool, Task> PeerHeadsetMuteChanged;

    /// <summary>
    ///     当有成员的语音权限或服务端静音状态变化时引发，成员可以是此客户端自己。
    /// </summary>
    /// <remarks>
    ///     管理员禁麦、禁听，或者变更角色权限时都会引发此事件。实测中，收回权限并不会导致客户端被断开。
    ///     <br />
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="System.UInt64"/> 参数是该成员的用户 ID。 </item>
    ///     <item> <see cref="Kook.Audio.WebRtc.VoicePeerPermissions"/> 参数是该成员变化后的权限。 </item>
    ///     </list>
    /// </remarks>
    event Func<ulong, VoicePeerPermissions, Task> PeerPermissionChanged;

    /// <summary>
    ///     当有成员使用氛围道具时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="System.UInt64"/> 参数是使用氛围道具的成员的用户 ID。 </item>
    ///     <item> <see cref="System.Nullable{T}"/> 的 <see cref="System.UInt64"/> 参数是氛围道具的 ID，服务端没有给出时为 <c>null</c>。 </item>
    ///     </list>
    /// </remarks>
    event Func<ulong, ulong?, Task> AtmosphereReceived;

    /// <summary>
    ///     当有成员开始播放伴奏时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="System.UInt64"/> 参数是播放伴奏的成员的用户 ID。 </item>
    ///     <item> <see cref="Kook.Audio.WebRtc.VoiceAccompaniment"/> 参数是伴奏的信息，其属性在服务端没有给出时为 <c>null</c>。 </item>
    ///     </list>
    /// </remarks>
    event Func<ulong, VoiceAccompaniment, Task> AccompanimentStarted;

    /// <summary>
    ///     当有成员停止播放伴奏时引发。
    /// </summary>
    /// <remarks>
    ///     此通知在实测中尚未出现过，其载荷格式是假定的，尚未验证。
    ///     <br />
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="System.UInt64"/> 参数是停止播放伴奏的成员的用户 ID。 </item>
    ///     </list>
    /// </remarks>
    event Func<ulong, Task> AccompanimentStopped;

    /// <summary>
    ///     当服务端通知此客户端被断开时引发，随后会引发 <see cref="Kook.Audio.IAudioClient.Disconnected"/> 事件。
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         实测中，下列三种情形服务端给出的 <c>errorCode</c> 都是 <c>41106</c>，原因只能靠 <c>errorReason</c> 区分，
    ///         而且其中一种的 <c>errorReason</c> 是空字符串：被管理员移出语音频道；同一个账号用 WebRTC 又连接了另一个频道，
    ///         先连上的 WebRTC 会话被断开；同一个账号用官方 UDP 方案加入了同一个频道，WebRTC 会话被断开（<c>errorReason</c> 为空）。
    ///         <c>errorReason</c> 是中文原文，不保证长期稳定，不要依赖它做精确匹配。
    ///         <c>errorCode</c> 还可能对应没有测试过的其他原因。
    ///     </note>
    ///     <br />
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="System.Nullable{T}"/> 的 <see cref="System.Int32"/> 参数是服务端给出的错误码，没有给出时为 <c>null</c>。 </item>
    ///     <item> <see cref="System.String"/> 参数是服务端给出的错误原因，没有给出时为 <c>null</c>。 </item>
    ///     </list>
    /// </remarks>
    event Func<int?, string?, Task> ServerDisconnected;

    /// <summary>
    ///     当服务端通知此客户端已在另一台设备上被关闭时引发，随后会引发 <see cref="Kook.Audio.IAudioClient.Disconnected"/> 事件。
    /// </summary>
    /// <remarks>
    ///     此通知在实测中尚未出现过，它的触发条件未知。名称提示它对应「同一账号在别处加入」，但实测中同一个账号用 WebRTC
    ///     连接另一个频道、或者用官方 UDP 方案加入同一个频道时，服务端走的都是 <c>disconnect</c> 通知，
    ///     会引发 <see cref="ServerDisconnected"/> 而不是此事件。
    /// </remarks>
    event Func<Task> ClosedByOtherDevice;

    /// <summary>
    ///     当服务端要求此客户端重新加入语音频道时引发，随后会引发 <see cref="Kook.Audio.IAudioClient.Disconnected"/> 事件。
    /// </summary>
    /// <remarks>
    ///     此客户端不支持自动重新加入，请在 <see cref="Kook.Audio.IAudioClient.Disconnected"/> 事件之后重新连接。
    ///     此通知在实测中尚未出现过。
    /// </remarks>
    event Func<Task> ReloadRequested;

    /// <summary>
    ///     获取此客户端所连接的语音频道的 ID。
    /// </summary>
    ulong ChannelId { get; }

    /// <summary>
    ///     获取一个值，指示此客户端的麦克风是否处于静音状态。
    /// </summary>
    bool IsMicrophoneMuted { get; }

    /// <summary>
    ///     设置此客户端的麦克风是否静音。
    /// </summary>
    /// <param name="muted"> 是否静音。 </param>
    /// <returns> 一个表示异步设置操作的任务。 </returns>
    Task SetMicrophoneMutedAsync(bool muted);

    /// <summary>
    ///     尝试获取指定 SSRC 的音频输入流所属的用户 ID。
    /// </summary>
    /// <param name="ssrc"> RTP 流的 SSRC 同步信源标识符。 </param>
    /// <param name="userId"> 如果找到，则为音频输入流所属的用户 ID；否则为 <c>0</c>。 </param>
    /// <returns> 是否找到了该 SSRC 所属的用户。 </returns>
    /// <remarks>
    ///     与基于 UDP 的官方语音接口不同，WebRTC 语音通道会在下发音频流时同时下发其所属的用户，
    ///     因此在 <see cref="Kook.Audio.IAudioClient.StreamCreated"/> 事件被引发时，即可通过此方法获取对应的用户。
    /// </remarks>
    bool TryGetStreamUserId(uint ssrc, out ulong userId);
}
