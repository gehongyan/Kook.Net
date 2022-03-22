namespace KaiHeiLa;

public interface IGuildUser : IUser
{
    #region General

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
    
    #endregion

    #region Permissions

    /// <summary>
    ///     Gets the guild-level permissions for this user.
    /// </summary>
    /// <returns>
    ///     A <see cref="KaiHeiLa.GuildPermissions"/> structure for this user, representing what
    ///     permissions this user has in the guild.
    /// </returns>
    GuildPermissions GuildPermissions { get; }
    
    /// <summary>
    ///     Gets the level permissions granted to this user to a given channel.
    /// </summary>
    /// <param name="channel">The channel to get the permission from.</param>
    /// <returns>
    ///     A <see cref="KaiHeiLa.ChannelPermissions"/> structure representing the permissions that a user has in the
    ///     specified channel.
    /// </returns>
    ChannelPermissions GetPermissions(IGuildChannel channel);

    #endregion

    #region Guild

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
    
    #endregion

    #region Roles
    
    /// <summary>
    ///     Adds the specified role to this user in the guild.
    /// </summary>
    /// <param name="roleId">The role to be added to the user.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous role addition operation.
    /// </returns>
    Task AddRoleAsync(uint roleId, RequestOptions options = null);
    /// <summary>
    ///     Adds the specified role to this user in the guild.
    /// </summary>
    /// <param name="role">The role to be added to the user.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous role addition operation.
    /// </returns>
    Task AddRoleAsync(IRole role, RequestOptions options = null);
    /// <summary>
    ///     Adds the specified <paramref name="roleIds"/> to this user in the guild.
    /// </summary>
    /// <param name="roleIds">The roles to be added to the user.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous role addition operation.
    /// </returns>
    Task AddRolesAsync(IEnumerable<uint> roleIds, RequestOptions options = null);
    /// <summary>
    ///     Adds the specified <paramref name="roles"/> to this user in the guild.
    /// </summary>
    /// <param name="roles">The roles to be added to the user.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous role addition operation.
    /// </returns>
    Task AddRolesAsync(IEnumerable<IRole> roles, RequestOptions options = null);
    /// <summary>
    ///     Removes the specified <paramref name="roleId"/> from this user in the guild.
    /// </summary>
    /// <param name="roleId">The role to be removed from the user.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous role removal operation.
    /// </returns>
    Task RemoveRoleAsync(uint roleId, RequestOptions options = null);
    /// <summary>
    ///     Removes the specified <paramref name="role"/> from this user in the guild.
    /// </summary>
    /// <param name="role">The role to be removed from the user.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous role removal operation.
    /// </returns>
    Task RemoveRoleAsync(IRole role, RequestOptions options = null);
    /// <summary>
    ///     Removes the specified <paramref name="roleIds"/> from this user in the guild.
    /// </summary>
    /// <param name="roleIds">The roles to be removed from the user.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous role removal operation.
    /// </returns>
    Task RemoveRolesAsync(IEnumerable<uint> roleIds, RequestOptions options = null);
    /// <summary>
    ///     Removes the specified <paramref name="roles"/> from this user in the guild.
    /// </summary>
    /// <param name="roles">The roles to be removed from the user.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous role removal operation.
    /// </returns>
    Task RemoveRolesAsync(IEnumerable<IRole> roles, RequestOptions options = null);
    
    #endregion
    
    #region Voice
    
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
    
    #endregion
}