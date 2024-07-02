namespace Kook;

/// <summary>
///     表示服务器中的一个通用语音频道。
/// </summary>
public interface IVoiceChannel : ITextChannel, IAudioChannel
{
    /// <summary>
    ///     获取要求语音频道中的客户端使用的语音质量。
    /// </summary>
    VoiceQuality? VoiceQuality { get; }

    /// <summary>
    ///     获取允许同时连接到此频道的最大用户数。
    /// </summary>
    /// <returns>
    ///     一个 <c>int</c>，表示允许同时连接到此频道的最大用户数；如果没有限制，则为 <c>0</c>。
    /// </returns>
    int UserLimit { get; }

    /// <summary>
    ///     获取此频道是否已被密码锁定。
    /// </summary>
    bool HasPassword { get; }

    /// <summary>
    ///     修改此语音频道。
    /// </summary>
    /// <param name="func"> 一个包含修改语音频道属性的委托。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     一个表示异步修改操作的任务。
    /// </returns>
    /// <seealso cref="T:Kook.ModifyVoiceChannelProperties"/>
    Task ModifyAsync(Action<ModifyVoiceChannelProperties> func, RequestOptions? options = null);

    /// <summary>
    ///     获取连接到此语音频道的用户。
    /// </summary>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     一个表示异步获取操作的任务。任务的结果包含连接到此语音频道的所有服务器用户。
    /// </returns>
    Task<IReadOnlyCollection<IUser>> GetConnectedUsersAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);
}
