namespace Kook;

/// <summary>
///     表示服务器中一个通用的帖子频道，可以浏览、发布和回复帖子。
/// </summary>
public interface IThreadChannel : INestedChannel, IMentionable
{
    /// <summary>
    ///     获取此频道的说明。
    /// </summary>
    string Topic { get; }

    /// <summary>
    ///     获取此频道设置的发帖速率限制。
    /// </summary>
    /// <remarks>
    ///     拥有 <see cref="Kook.ChannelPermission.ManageMessages"/> 或
    ///     <see cref="Kook.ChannelPermission.ManageChannels"/> 权限的用户不受慢速模式延迟的限制。
    /// </remarks>
    /// <returns> 一个 <c>int</c>，表示用户在可以发布另一条帖子之前需要等待的时间（以秒为单位）；如果未启用，则为 <c>0</c>。 </returns>
    int PostCreationInterval { get; }

    /// <summary>
    ///     获取此频道设置的回帖速率限制。
    /// </summary>
    /// <remarks>
    ///     拥有 <see cref="Kook.ChannelPermission.ManageMessages"/> 或
    ///     <see cref="Kook.ChannelPermission.ManageChannels"/> 权限的用户不受慢速模式延迟的限制。
    /// </remarks>
    /// <returns> 一个 <c>int</c>，表示用户在可以对任意帖子发布另一条回复之前需要等待的时间（以秒为单位）；如果未启用，则为 <c>0</c>。 </returns>
    int? ReplyInterval { get; }

    /// <summary>
    ///     获取此频道设置的帖子默认布局。
    /// </summary>
    ThreadChannelLayout? DefaultLayout { get; }

    /// <summary>
    ///     获取此频道设置的帖子默认排序。
    /// </summary>
    ThreadSortMode? DefaultSortMode { get; }

    /// <summary>
    ///     修改此频道的属性。
    /// </summary>
    /// <param name="func"> 一个包含修改帖子频道的属性的委托。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步修改操作的任务。 </returns>
    /// <seealso cref="Kook.ModifyThreadChannelProperties"/>
    Task ModifyAsync(Action<ModifyThreadChannelProperties> func, RequestOptions? options = null);
}
