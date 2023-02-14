namespace Kook;

public interface IUser : IEntity<ulong>, IMentionable, IPresence
{
    /// <summary>
    ///     Gets the username for this user.
    /// </summary>
    string Username { get; }
    /// <summary>
    ///     Gets the per-username unique ID for this user.
    /// </summary>
    string IdentifyNumber { get; }
    /// <summary>
    ///     Gets the per-username unique ID for this user.
    /// </summary>
    ushort? IdentifyNumberValue { get; }
    /// <summary>
    ///     Gets whether this user is a bot; <c>null</c> if unknown.
    /// </summary>
    bool? IsBot { get; }
    /// <summary>
    ///     Gets whether this user is banned; <c>null</c> if unknown.
    /// </summary>
    bool? IsBanned { get; }
    /// <summary>
    ///     Gets whether this user has subscribed to BUFF; <c>null</c> if unknown.
    /// </summary>
    bool? HasBuff { get; }
    /// <summary>
    ///     Gets the link to this user's avatar.
    /// </summary>
    string Avatar { get; }
    /// <summary>
    ///     Gets the link to this user's BUFF avatar.
    /// </summary>
    string BuffAvatar { get; }
    /// <summary>
    ///     Gets the link to this user's banner.
    /// </summary>
    string Banner { get; }
    /// <summary>
    ///     Gets whether this user enabled denoise feature; <c>null</c> if unknown.
    /// </summary>
    bool? IsDenoiseEnabled { get; }
    /// <summary>
    ///     Get the tag this user has.
    /// </summary>
    UserTag UserTag { get; }

    /// <summary>
    ///     Creates the direct message channel of this user.
    /// </summary>
    /// <remarks>
    ///     This method is used to obtain or create a channel used to send a direct message.
    ///     <note type="warning">
    ///          In event that the current user cannot send a message to the target user, a channel can and will
    ///          still be created by Kook. However, attempting to send a message will yield a
    ///          <see cref="Kook.Net.HttpException"/> with a 403 as its
    ///          <see cref="Kook.Net.HttpException.HttpCode"/>. There are currently no official workarounds by
    ///          Kook.
    ///     </note>
    /// </remarks>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation for getting or creating a DM channel. The task result
    ///     contains the DM channel associated with this user.
    /// </returns>
    Task<IDMChannel> CreateDMChannelAsync(RequestOptions options = null);

    /// <summary>
    ///     Gets the intimacy information with this user.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation for getting the intimacy information. The task result
    ///     contains the intimacy information associated with this user.
    /// </returns>
    Task<IIntimacy> GetIntimacyAsync(RequestOptions options = null);

    /// <summary>
    ///     Updates the intimacy information with this user.
    /// </summary>
    /// <param name="func">A delegate containing the properties to modify the <see cref="IIntimacy"/> with.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>A task that represents the asynchronous operation for updating the intimacy information.</returns>
    Task UpdateIntimacyAsync(Action<IntimacyProperties> func, RequestOptions options);
}
