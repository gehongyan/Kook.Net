namespace Kook;

/// <summary>
///     Represents a generic voice channel in a guild.
/// </summary>
public interface IVoiceChannel : ITextChannel, IAudioChannel
{
    /// <summary>
    ///     Gets the voice quality that the clients in this voice channel are requested to use.
    /// </summary>
    /// <returns>
    ///     A <see cref="VoiceQuality"/> representing the voice quality that this voice channel defines and requests the
    ///     client(s) to use.
    /// </returns>
    VoiceQuality? VoiceQuality { get; }

    /// <summary>
    ///     Gets the max number of users allowed to be connected to this channel at once.
    /// </summary>
    /// <returns>
    ///     An int representing the maximum number of users that are allowed to be connected to this
    ///     channel at once; <c>0</c> if a limit is not set.
    /// </returns>
    int UserLimit { get; }

    /// <summary>
    ///     Gets whether this voice channel is locked by a password.
    /// </summary>
    /// <returns>
    ///     A bool representing whether this voice channel is locked by a password.
    /// </returns>
    bool HasPassword { get; }

    /// <summary>
    ///     Modifies this voice channel.
    /// </summary>
    /// <param name="func">The properties to modify the channel with.</param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     A task that represents the asynchronous modification operation.
    /// </returns>
    /// <seealso cref="ModifyVoiceChannelProperties"/>
    Task ModifyAsync(Action<ModifyVoiceChannelProperties> func, RequestOptions? options = null);

    /// <summary>
    ///     Gets the users connected to this voice channel.
    /// </summary>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a collection of
    ///     <see cref="IGuildUser"/>s that are connected to this voice channel.
    /// </returns>
    Task<IReadOnlyCollection<IUser>> GetConnectedUsersAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);
}
