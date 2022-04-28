namespace KaiHeiLa;

public interface IUser : IEntity<ulong>, IMentionable
{
    string Username { get; }

    string IdentifyNumber { get; }

    ushort? IdentifyNumberValue { get; }

    bool? IsOnline { get; }

    bool? IsBot { get; }

    bool? IsBanned { get; }

    bool? IsVIP { get; }

    string Avatar { get; }

    string VIPAvatar { get; }

    bool? IsDenoiseEnabled { get; }
    
    UserTag UserTag { get; }

    /// <summary>
    ///     Creates the direct message channel of this user.
    /// </summary>
    /// <remarks>
    ///     This method is used to obtain or create a channel used to send a direct message.
    ///     <note type="warning">
    ///          In event that the current user cannot send a message to the target user, a channel can and will
    ///          still be created by Discord. However, attempting to send a message will yield a
    ///          <see cref="KaiHeiLa.Net.HttpException"/> with a 403 as its
    ///          <see cref="KaiHeiLa.Net.HttpException.HttpCode"/>. There are currently no official workarounds by
    ///          KaiHeiLa.
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