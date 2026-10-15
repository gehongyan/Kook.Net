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
///     <para>
///         <b>同时存在多个语音连接时的行为。</b>
///         <note type="warning">
///             下列行为均来自对同一个 Bot 账号的实测，每种组合只测过少数几次，不是 KOOK 的文档化保证，之后可能变化。
///         </note>
///         <list type="bullet">
///         <item>
///             同一个账号同一时间只能有一个 WebRTC 会话。用 WebRTC 连接第二个频道（即使在另一个服务器）时，
///             先连接的客户端会被服务端断开，它会先引发 <see cref="ServerDisconnected"/>（错误码 <c>41106</c>），
///             随后引发 <see cref="Kook.Audio.IAudioClient.Disconnected"/>。
///         </item>
///         <item>
///             官方的基于 UDP 的语音方案（<see cref="Kook.WebSocket.SocketVoiceChannel.ConnectAsync(System.Boolean,System.Boolean,System.String)"/>）
///             可以让同一个账号同时连接多个频道，不受上一条限制。需要让一个 Bot 同时在多个频道里推流时，请使用 UDP 方案。
///         </item>
///         <item>
///             UDP 与 WebRTC 在不同的频道里可以同时存在，互不影响。在同一个频道里则会冲突，UDP 优先：
///             UDP 已经连接时，WebRTC 的加入会被服务端拒绝；WebRTC 已经连接时，随后加入的 UDP 会让它被断开
///             （<see cref="ServerDisconnected"/> 的错误码为 <c>41106</c>，错误原因为空）。
///         </item>
///         </list>
///     </para>
///     <para>
///         <b>权限。</b>
///         Bot 需要被授权才能连接语音频道。实测中，一个没有任何服务器角色的 Bot 获取语音网关令牌时会被服务端拒绝（HTTP 403），
///         授予角色后可以正常加入。Bot 无法访问频道所属的服务器时（例如不在该服务器，或者该服务器已不可用），
///         服务端返回 HTTP 400，错误信息为 <c>guild_id does not exist or you do not have permission to access.</c>，
///         不区分「不存在」和「没有权限」。
///     </para>
///     <para>
///         <b>发送音频。</b>
///         <list type="bullet">
///         <item> 写入音频流的 PCM 应为 48 kHz、16 位、双声道交错。 </item>
///         <item>
///             编码码率默认取服务端在加入时下发的上限，实测在不同频道里见过 32 kbps 和 64 kbps。每个 Opus 包的时长为 60 毫秒。
///         </item>
///         <item>
///             <see cref="Kook.Audio.IAudioClient.CreatePcmStream(Kook.Audio.AudioApplication,System.Nullable{System.Int32},System.Int32,System.Int32)"/>
///             的 <c>packetLoss</c> 参数默认为 <c>30</c>。此客户端在 <c>packetLoss</c> 大于 <c>0</c> 时会启用 Opus 带内 FEC，
///             并为丢包补偿预留码率。在 32 kbps 下对同一段音乐做的离线对比（Concentus 编码后再解码）中，
///             预期丢包 10% 以上并启用 FEC 会使信噪比下降约 2.4 dB，高频能量只剩约一半。网络状况良好时，建议传入
///             <c>packetLoss: 0</c>，此时不会启用 FEC。
///         </item>
///         <item> 麦克风处于静音状态时，写入音频流的音频会被丢弃，不会缓存到取消静音之后。 </item>
///         </list>
///     </para>
///     <para>
///         <b>接收音频。</b>
///         <list type="bullet">
///         <item>
///             只有开着麦克风的成员才有音频流。成员静音入会、还没有开过麦时没有音频流，他第一次开麦时才会创建。
///         </item>
///         <item>
///             成员开关麦克风时，服务端下发的是静音状态变化的通知（<see cref="PeerMicrophoneMuteChanged"/>），实测中他的音频流并没有被销毁。
///             成员离开频道后再加入，会得到新的音频流和新的 SSRC，同一个用户在两次会话里的 SSRC 不同。
///             因此应当以用户 ID（<see cref="TryGetStreamUserId(System.UInt32,out System.UInt64)"/>）而不是 SSRC 来识别说话的人。
///         </item>
///         <item>
///             <see cref="Kook.Audio.AudioInStream"/> 提供的是解码后的 48 kHz、16 位、双声道 PCM；
///             对方使用单声道的设备（实测的 Android 客户端）时，同样会被解码为双声道。每一帧的时长取决于对方的设备。
///         </item>
///         <item>
///             没有被读取的音频流最多缓存 100 帧（约 1 到 2 秒），缓冲区满之后新到的帧会被直接丢弃，所以不会无限占用内存；
///             需要这些音频的话，请及时读取。
///         </item>
///         </list>
///     </para>
///     <para>
///         <b>不支持自动重连。</b>
///         服务端断开（<see cref="ServerDisconnected"/>、<see cref="ClosedByOtherDevice"/>、<see cref="ReloadRequested"/>）
///         或信令连接关闭后，客户端会引发 <see cref="Kook.Audio.IAudioClient.Disconnected"/>，需要调用方自行重新连接。
///         通过 <c>ConnectWebRtcAsync</c> 创建的客户端不会赋给 <see cref="Kook.WebSocket.SocketVoiceChannel.AudioClient"/>，
///         需要调用方自己持有，并在不再使用时释放。
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
