namespace Kook.Audio;

/// <summary>
///     表示一个通用的音频客户端。
/// </summary>
public interface IAudioClient : IDisposable
{
    /// <summary>
    ///     Occurs when a new incoming stream is created.
    ///     The first <see cref="int"/> parameter is the RTP SSRC.
    ///     The second <see cref="AudioInStream" /> parameter is the audio stream.
    /// </summary>
    event Func<uint, AudioInStream, Task> StreamCreated;

    /// <summary>
    ///     Occurs when an incoming stream is destroyed.
    ///     The first <see cref="int"/> parameter is the RTP SSRC.
    /// </summary>
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
    ///     <item> <see cref="T:System.Int32"/> 参数是当前更新后的延迟（毫秒）。 </item>
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
