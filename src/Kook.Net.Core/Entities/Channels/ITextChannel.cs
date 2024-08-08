namespace Kook;

/// <summary>
///     表示服务器中一个通用的具有文字聊天能力的频道，可以发送和接收消息。
/// </summary>
public interface ITextChannel : INestedChannel, IMentionable, IMessageChannel
{
    #region General

    /// <summary>
    ///     获取此频道的说明。
    /// </summary>
    string Topic { get; }

    /// <summary>
    ///     获取此频道当前设置的慢速模式延迟。
    /// </summary>
    /// <remarks>
    ///     拥有 <see cref="F:Kook.ChannelPermission.ManageMessages"/> 或
    ///     <see cref="F:Kook.ChannelPermission.ManageChannels"/> 权限的用户不受慢速模式延迟的限制。
    /// </remarks>
    /// <returns> 一个 <c>int</c>，表示用户在可以发送另一条消息之前需要等待的时间（以秒为单位）；如果未启用，则为 <c>0</c>。 </returns>
    int SlowModeInterval { get; }

    /// <summary>
    ///     修改此频道有关文字聊天能力的属性。
    /// </summary>
    /// <param name="func"> 一个包含修改频道有关文字聊天能力的属性的委托。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步修改操作的任务。 </returns>
    /// <seealso cref="T:Kook.ModifyTextChannelProperties"/>
    Task ModifyAsync(Action<ModifyTextChannelProperties> func, RequestOptions? options = null);

    #endregion

    /// <summary>
    ///     获取此频道中的所有置顶消息。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含此频道中找到的所有置顶消息。 </returns>
    Task<IReadOnlyCollection<IMessage>> GetPinnedMessagesAsync(RequestOptions? options = null);
}
