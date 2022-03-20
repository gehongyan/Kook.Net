namespace KaiHeiLa;

/// <summary>
///     文字频道
/// </summary>
public interface ITextChannel : INestedChannel, IMentionable, IMessageChannel
{
    #region General

    /// <summary>
    ///     频道简介
    /// </summary>
    string Topic { get; }
    
    /// <summary>
    ///     慢速模式下限制发言的最短时间间隔, 单位为秒(s)
    /// </summary>
    int SlowModeInterval { get; }

    #endregion
    
    /// <summary>
    ///     Gets a collection of pinned messages in this channel.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation for retrieving pinned messages in this channel.
    ///     The task result contains a collection of messages found in the pinned messages.
    /// </returns>
    Task<IReadOnlyCollection<IMessage>> GetPinnedMessagesAsync(RequestOptions options = null);
}