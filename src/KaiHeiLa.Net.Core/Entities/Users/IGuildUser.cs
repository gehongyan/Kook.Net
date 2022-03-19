namespace KaiHeiLa;

public interface IGuildUser : IUser
{
    /// <summary>
    ///     用户在当前服务器的昵称
    /// </summary>
    string Nickname { get; }

    /// <summary>
    ///     用户在当前服务器中的角色 id 组成的列表
    /// </summary>
    IReadOnlyCollection<uint> RoleIds { get; }

    /// <summary>
    ///     此服务器用户所属服务器
    /// </summary>
    IGuild Guild { get; }

    /// <summary>
    ///     此服务器用户所属服务器的ID
    /// </summary>
    ulong GuildId { get; }

    bool IsMobileVerified { get; }
    
    DateTimeOffset JoinedAt { get; }
    
    DateTimeOffset ActiveAt { get; }
    
    Color Color { get; }
    
    bool? IsOwner { get; }
    
    /// <summary>
    ///     Kicks this user from this guild.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous kick operation.
    /// </returns>
    Task KickAsync(RequestOptions options = null);

    /// <summary>
    ///     Modifies this user's nickname in this guild.
    /// </summary>
    /// <remarks>
    ///     This method modifies the nickname of current guild user.
    /// </remarks>
    /// <param name="func">The delegate containing the nickname to modify the user with.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous modification operation.
    /// </returns>
    Task ModifyNicknameAsync(Action<string> func, RequestOptions options = null);
    
    /// <summary>
    ///     Mute this user in this guild.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous muting operation.
    /// </returns>
    Task MuteAsync(RequestOptions options = null);
    
    /// <summary>
    ///     Deafen this user in this guild.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous deafening operation.
    /// </returns>
    Task DeafenAsync(RequestOptions options = null);
    
    /// <summary>
    ///     Unmute this user in this guild.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous unmuting operation.
    /// </returns>
    Task UnmuteAsync(RequestOptions options = null);
    
    /// <summary>
    ///     Undeafen this user in this guild.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous undeafening operation.
    /// </returns>
    Task UndeafenAsync(RequestOptions options = null);
    
    /// <summary>
    ///     Gets a collection of voice channels a user
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<IVoiceChannel>> GetConnectedVoiceChannelAsync(RequestOptions options = null);
}