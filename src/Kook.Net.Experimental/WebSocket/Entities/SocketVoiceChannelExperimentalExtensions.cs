using Kook.WebSocket;

namespace Kook.Audio.WebRtc;

/// <summary>
///     提供用于 <see cref="Kook.WebSocket.SocketVoiceChannel"/> 的实验性 WebRTC 语音功能的扩展方法。
/// </summary>
public static class SocketVoiceChannelExperimentalExtensions
{
    private const int ConnectTimeoutMilliseconds = 15000;

    /// <summary>
    ///     通过 WebRTC 语音通道连接到此语音频道。
    /// </summary>
    /// <param name="channel"> 要连接的语音频道。 </param>
    /// <param name="configure">
    ///     在开始连接之前调用的回调，用于订阅音频客户端的事件。
    ///     连接过程中，已在频道内发言的用户的音频流会被立即创建，如果在连接完成后才订阅
    ///     <see cref="Kook.Audio.IAudioClient.StreamCreated"/>，将会错过这些音频流的创建事件。
    /// </param>
    /// <returns> 一个表示异步连接操作的任务，任务的结果是已连接的音频客户端。 </returns>
    /// <remarks>
    ///     此方法使用 KOOK 非官方文档化的语音通道，行为可能随时变化。使用 <see cref="Kook.Audio.IAudioClient.StopAsync"/>
    ///     与 <see cref="System.IDisposable.Dispose"/> 断开连接并释放资源。
    /// </remarks>
    /// <exception cref="System.TimeoutException"> 音频客户端未能在 15 秒内连接成功。 </exception>
    public static async Task<IWebRtcAudioClient> ConnectWebRtcAsync(this SocketVoiceChannel channel,
        Action<IWebRtcAudioClient>? configure = null)
    {
        Preconditions.NotNull(channel, nameof(channel));

        TaskCompletionSource<WebRtcAudioClient> promise = new();
        WebRtcAudioClient client = new(channel, channel.Kook.GetAudioId());
        client.Disconnected += ex =>
        {
            if (!promise.Task.IsCompleted && client.IsFinished)
                promise.TrySetException(ex);
            return Task.CompletedTask;
        };
        client.Connected += () =>
        {
            promise.TrySetResult(client);
            return Task.CompletedTask;
        };

        try
        {
            configure?.Invoke(client);
            await client.StartAsync().ConfigureAwait(false);

            Task timeoutTask = Task.Delay(ConnectTimeoutMilliseconds);
            Task completedTask = await Task.WhenAny(promise.Task, timeoutTask).ConfigureAwait(false);
            if (completedTask == timeoutTask)
                throw new TimeoutException("The audio client failed to connect within 15 seconds.");
            return await promise.Task.ConfigureAwait(false);
        }
        catch
        {
            try
            {
                client.Dispose();
            }
            catch
            {
                // ignored
            }
            throw;
        }
    }
}
