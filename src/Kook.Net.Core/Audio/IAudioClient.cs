namespace Kook.Audio;

/// <summary>
///     表示一个通用的音频客户端。
/// </summary>
public interface IAudioClient : IDisposable
{
    /// <summary>
    ///     当客户端接收到一个新的语音输入流时引发。
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         SSRC 为 RTP 实时传输协议中的同步信源标识符，RTP 会话中，每个媒体流应具有一个唯一的 SSRC 标识符。KOOK
    ///         服务端未通过网关下发用户与 RTP 流 SSRC 的映射关系，因此无法通过 SSRC 直接获取其关联的用户。Kook.Net 遵循 RTP
    ///         协议，以 SSRC 区分不同的信源，创建不同的 <see cref="T:Kook.Audio.AudioInStream"/> 实例，并引发此事件。
    ///         一般地，每个 SSRC 值都可以分别表示一个用户在一个语音频道内在一次连接与断开之间的音频流的唯一标识符，
    ///         同一用户切换语音频道或断开后重新连接到同一语音频道，KOOK 语音服务器都会为其分配新的 SSRC 标识符。
    ///     </note>
    ///     <br />
    ///     <note type="warning">
    ///         Bot 用户可以通过 API 指定要使用的 SSRC 标识符，这可能会导致 SSRC 碰撞。KOOK 开发者文档推荐 Bot
    ///         用户使用 <c>1111</c> 为 SSRC 标识符，如果需要区分音频流是否由 Bot 创建，可以尝试判断 SSRC 标识符是否为
    ///         <c>1111</c> 作为参考。Kook.Net 不以固定的 <c>1111</c> 作为 SSRC
    ///         标识符，而是使用随机值，如果音频由 Kook.Net 所构建的 Bot 推送，此方法可能无法区分该音频流是否由 Bot 创建。
    ///     </note>
    ///     <br />
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:System.Int32"/> 参数是 RTP 流的 SSRC 同步信源标识符。 </item>
    ///     <item> <see cref="T:Kook.Audio.AudioInStream"/> 参数是所新创建的音频输入流。 </item>
    ///     </list>
    /// </remarks>
    event Func<uint, AudioInStream, Task> StreamCreated;

    /// <summary>
    ///     当音频输入流被销毁时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="T:System.Int32"/> 参数是 RTP 流的 SSRC 同步信源标识符。 </item>
    ///     </list>
    /// </remarks>
    event Func<uint, Task> StreamDestroyed;

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
    ///     <item> <see cref="T:System.Int32"/> 参数是更新后的延迟（毫秒）。 </item>
    ///     </list>
    /// </remarks>
    event Func<int, int, Task> UdpLatencyUpdated;

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
    ///     获取该语音客户端所接收的所有音频输入流。
    /// </summary>
    /// <returns> 一个只读字典，其中键是 RTP 流的 SSRC 同步信源标识符，值是对应的音频输入流。 </returns>
    IReadOnlyDictionary<uint, AudioInStream> GetStreams();

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
    /// <param name="bitrate"> 音频流的比特率，单位为比特每秒；留空则使用在请求建立音频通道时由 KOOK 返回的指定的比特率。 </param>
    /// <param name="bufferMillis"> 音频流的缓冲区大小，单位为毫秒。 </param>
    /// <param name="packetLoss"> 音频流的丢包率，单位为百分比，默认值为 30%。 </param>
    /// <returns> 一个新的 PCM 音频流。 </returns>
    /// <remarks>
    ///     <note type="warning">
    ///         如果为 <paramref name="bitrate" /> 设置了比特率，但比特率过多地超过了由
    ///         KOOK 返回的指定的比特率，语音连接可能会被 KOOK 语音服务断开。
    ///     </note>
    /// </remarks>
    AudioOutStream CreatePcmStream(AudioApplication application, int? bitrate = null, int bufferMillis = 1000, int packetLoss = 30);

    /// <summary>
    ///     使用 PCM 编解码器创建一个新的音频流，不设置缓冲区。
    /// </summary>
    /// <param name="application"> 音频应用程序的应用场景。 </param>
    /// <param name="bitrate"> 音频流的比特率，单位为比特每秒；留空则使用在请求建立音频通道时由 KOOK 返回的指定的比特率。 </param>
    /// <param name="packetLoss"> 音频流的丢包率，单位为百分比，默认值为 30%。 </param>
    /// <returns> 一个新的 PCM 音频流。 </returns>
    /// <remarks>
    ///     <note type="warning">
    ///         如果为 <paramref name="bitrate" /> 设置了比特率，但比特率过多地超过了由
    ///         KOOK 返回的指定的比特率，语音连接可能会被 KOOK 语音服务断开。
    ///     </note>
    /// </remarks>
    AudioOutStream CreateDirectPcmStream(AudioApplication application, int? bitrate = null, int packetLoss = 30);
}
