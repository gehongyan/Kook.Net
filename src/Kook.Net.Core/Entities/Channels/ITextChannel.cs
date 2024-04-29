namespace Kook;

/// <summary>
///     Represents a generic channel in a guild that can send and receive messages.
/// </summary>
public interface ITextChannel : INestedChannel, IMentionable, IMessageChannel
{
    #region General

    /// <summary>
    ///     Gets the current topic for this text channel.
    /// </summary>
    /// <returns>
    ///     A string representing the topic set in the channel; <c>null</c> if none is set.
    /// </returns>
    string Topic { get; }

    /// <summary>
    ///     Gets the current slow-mode delay for this channel.
    /// </summary>
    /// <returns>
    ///     An int representing the time in seconds required before the user can send another message; 0 if disabled.
    /// </returns>
    int SlowModeInterval { get; }

    /// <summary>
    ///     Modifies this text channel.
    /// </summary>
    /// <param name="func">The delegate containing the properties to modify the channel with.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous modification operation.
    /// </returns>
    /// <seealso cref="ModifyTextChannelProperties"/>
    Task ModifyAsync(Action<ModifyTextChannelProperties> func, RequestOptions? options = null);

    #endregion

    /// <summary>
    ///     Gets a collection of pinned messages in this channel.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation for retrieving pinned messages in this channel.
    ///     The task result contains a collection of messages found in the pinned messages.
    /// </returns>
    Task<IReadOnlyCollection<IMessage>> GetPinnedMessagesAsync(RequestOptions? options = null);
}
