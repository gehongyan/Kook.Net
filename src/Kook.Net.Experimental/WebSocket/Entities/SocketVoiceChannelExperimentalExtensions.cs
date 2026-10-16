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
    ///     <para>
    ///         此方法使用 KOOK 非官方文档化的语音通道，行为可能随时变化。使用 <see cref="Kook.Audio.IAudioClient.StopAsync"/>
    ///         与 <see cref="System.IDisposable.Dispose"/> 断开连接并释放资源。
    ///     </para>
    ///     <para>
    ///         返回的客户端不会赋给 <see cref="Kook.WebSocket.SocketVoiceChannel.AudioClient"/>，该属性仍然只表示基于 UDP 的官方语音连接，
    ///         因此需要调用方自己持有返回的客户端。此方法不会断开该频道上已有的 UDP 连接，
    ///         但同一个频道里 UDP 与 WebRTC 不能同时在线，同一个账号同一时间也只能有一个 WebRTC 会话，详见
    ///         <see cref="Kook.Audio.WebRtc.IWebRtcAudioClient"/> 的说明。
    ///     </para>
    ///     <para>
    ///         连接失败时，下列情形是实测中见过的：
    ///         <list type="bullet">
    ///         <item>
    ///             获取语音网关令牌时被服务端拒绝，会抛出 <see cref="Kook.Net.HttpException"/>。实测中，没有任何服务器角色的 Bot
    ///             得到的是 HTTP 403（<c>你没有权限进行此操作</c>）。Bot 无法访问该频道所属的服务器时，服务端返回的是 HTTP 400，
    ///             这一种情形只用 REST 探针确认过，没有经过 .NET 代码验证，异常类型是按同一条请求路径推断的。
    ///         </item>
    ///         <item>
    ///             服务端在加入时拒绝了请求，例如同一个频道里已经有该账号的 UDP 连接（实测为 <c>401 peerId error!</c>），
    ///             会抛出异常，其类型目前没有公开，只能通过 <see cref="System.Exception"/> 捕获，错误信息中包含服务端给出的原因。
    ///         </item>
    ///         <item> 超过 15 秒仍未连接成功，会抛出 <see cref="System.TimeoutException"/>。 </item>
    ///         </list>
    ///         连接失败时，此方法已经释放了所创建的客户端，调用方不需要再处理。
    ///     </para>
    /// </remarks>
    /// <exception cref="Kook.Net.HttpException"> 获取语音网关令牌失败，例如 Bot 没有权限。 </exception>
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
