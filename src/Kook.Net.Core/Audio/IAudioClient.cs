namespace Kook.Audio;

/// <summary>
///     表示一个通用的音频客户端。
/// </summary>
public interface IAudioClient : IDisposable
{
    /// <summary>
    ///     当客户端成功连接到语音服务器时引发。
    /// </summary>
    event Func<Task> Connected;

    /// <summary>
    ///     当客户端从语音服务器断开连接时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:System.Exception"/> 参数是导致连接断开的异常。 </item>
    ///     </list>
    /// </remarks>
    event Func<Exception, Task> Disconnected;

    // /// <summary>
    // ///     当语音 WebSocket 服务器的延迟更新时引发。
    // /// </summary>
    // event Func<int, int, Task> LatencyUpdated;

    /// <summary>
    ///     当语音 UDP 服务器的延迟已更新时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:System.Int32"/> 参数是更新前的延迟（毫秒）。 </item>
    ///     <item> <see cref="T:System.Int32"/> 参数是当前更新后的延迟（毫秒）。 </item>
    ///     </list>
    /// </remarks>
    event Func<int, int, Task> UdpLatencyUpdated;

    /// <summary>
    ///     当其他语音客户端连接到当前语音服务器时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:System.UInt64"/> 参数是连接到语音服务器的语音客户端用户的 ID。 </item>
    ///     </list>
    /// </remarks>
    event Func<ulong, Task> PeerConnected;

    /// <summary>
    ///     当其他语音客户端从当前语音服务器断开连接时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:System.UInt64"/> 参数是断开连接的语音客户端用户的 ID。 </item>
    ///     </list>
    /// </remarks>
    event Func<ulong, Task> PeerDisconnected;

    /// <summary>
    ///     当其他语音客户端已恢复发送语音数据时引发，通常是闭麦状态解除操作。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:System.UInt64"/> 参数是恢复发送语音数据的语音客户端用户的 ID。 </item>
    ///     </list>
    /// </remarks>
    event Func<ulong, Task> HeadsetResumed;

    /// <summary>
    ///     当其他语音客户端已暂停发送语音数据时引发，通常是闭麦操作。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:System.UInt64"/> 参数是暂停发送语音数据的语音客户端用户的 ID。 </item>
    ///     </list>
    /// </remarks>
    event Func<ulong, Task> HeadsetPaused;

    /// <summary>
    ///     当其他语音客户端已恢复接收语音数据时引发，通常是静音状态解除操作。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:System.UInt64"/> 参数是恢复接收语音数据的语音客户端用户的 ID。 </item>
    ///     </list>
    /// </remarks>
    event Func<ulong, Task> ConsumerResumed;

    /// <summary>
    ///     当其他语音客户端已暂停接收语音数据时引发，通常是静音操作。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:System.UInt64"/> 参数是暂停接收语音数据的语音客户端用户的 ID。 </item>
    ///     </list>
    /// </remarks>
    event Func<ulong, Task> ConsumerPaused;

    /// <summary>
    ///     当其他语音客户端的权限发生变化时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:System.UInt64"/> 参数是权限发生变化的语音客户端用户的 ID。 </item>
    ///     <item> <see cref="T:Kook.Audio.PeerPermissionInfo"/> 参数是变更后的权限信息。 </item>
    ///     </list>
    /// </remarks>
    event Func<ulong, PeerPermissionInfo, Task> PeerPermissionChanged;

    /// <summary>
    ///     当其他语音客户端播放了一个氛围音效时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:System.UInt64"/> 参数是播放氛围音效的语音客户端用户的 ID。 </item>
    ///     <item> <see cref="T:Kook.Audio.PeerPermissionInfo"/> 参数是氛围音效的 ID。 </item>
    ///     </list>
    /// </remarks>
    event Func<ulong, int, Task> AtmospherePlayed;

    /// <summary>
    ///     当其他语音客户端开始了共享来自计算机其它应用程序的实时音频时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:System.UInt64"/> 参数是开始共享计算机应用程序音频的语音客户端用户的 ID。 </item>
    ///     </list>
    /// </remarks>
    event Func<ulong, SoundtrackInfo, Task> SoundtrackStarted;

    /// <summary>
    ///     当其他语音客户端停止了共享来自计算机其它应用程序的实时音频时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:System.UInt64"/> 参数是停止共享计算机应用程序音频的语音客户端用户的 ID。 </item>
    ///     </list>
    /// </remarks>
    event Func<ulong, Task> SoundtrackStopped;

    /// <summary>
    ///     当此语音客户端从语音服务器断开连接时引发。
    /// </summary>
    event Func<Task> ClientDisconnected;

    /// <summary>
    ///     获取此客户端当前的连接状态。
    /// </summary>
    ConnectionState ConnectionState { get; }

    // /// <summary>
    // ///     获取到语音 WebSocket 服务器的往返时间估计值，单位为毫秒。
    // /// </summary>
    // int Latency { get; }

    /// <summary>
    ///     获取到语音 UDP 服务器的往返时间估计值，单位为毫秒。
    /// </summary>
    int UdpLatency { get; }

    /// <summary>
    ///     停止该语音客户端发送音频。
    /// </summary>
    /// <returns> 一个停止操作的异步任务。 </returns>
    Task StopAsync();

    /// <summary>
    ///     使用 Opus 编解码器创建一个新的音频流。
    /// </summary>
    /// <param name="bufferMillis"> 音频流的缓冲区大小，单位为毫秒。 </param>
    /// <returns> 一个新的 Opus 音频流。 </returns>
    AudioOutStream CreateOpusStream(int bufferMillis = 1000);

    /// <summary>
    ///     使用 Opus 编解码器创建一个新的音频流，不设置缓冲区。
    /// </summary>
    /// <returns> 一个新的 Opus 音频流。 </returns>
    AudioOutStream CreateDirectOpusStream();

    /// <summary>
    ///     使用 PCM 编解码器创建一个新的音频流。
    /// </summary>
    /// <param name="application"> 音频应用程序的应用场景。 </param>
    /// <param name="bitrate"> 音频流的比特率，单位为比特每秒，默认值为 96 kbps。 </param>
    /// <param name="bufferMillis"> 音频流的缓冲区大小，单位为毫秒。 </param>
    /// <param name="packetLoss"> 音频流的丢包率，单位为百分比，默认值为 30%。 </param>
    /// <returns> 一个新的 PCM 音频流。 </returns>
    AudioOutStream CreatePcmStream(AudioApplication application, int bitrate = 96 * 1024, int bufferMillis = 1000, int packetLoss = 30);

    /// <summary>
    ///     使用 PCM 编解码器创建一个新的音频流，不设置缓冲区。
    /// </summary>
    /// <param name="application"> 音频应用程序的应用场景。 </param>
    /// <param name="bitrate"> 音频流的比特率，单位为比特每秒，默认值为 96 kbps。 </param>
    /// <param name="packetLoss"> 音频流的丢包率，单位为百分比，默认值为 30%。 </param>
    /// <returns> 一个新的 PCM 音频流。 </returns>
    AudioOutStream CreateDirectPcmStream(AudioApplication application, int bitrate = 96 * 1024, int packetLoss = 30);
}
