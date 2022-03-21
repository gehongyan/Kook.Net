﻿namespace KaiHeiLa;

public interface IGuild : IEntity<ulong>
{
    #region General

    string Name { get; }

    string Topic { get; }

    ulong OwnerId { get; }

    string Icon { get; }

    NotifyType NotifyType { get; }

    string Region { get; }

    bool IsOpenEnabled { get; }

    uint OpenId { get; }

    ulong DefaultChannelId { get; }

    ulong WelcomeChannelId { get; }
    
    /// <summary>
    ///     Determines if this guild is currently connected and ready to be used.
    /// </summary>
    /// <remarks>
    ///     <note>
    ///         This property only applies to a WebSocket-based client.
    ///     </note>
    ///     This boolean is used to determine if the guild is currently connected to the WebSocket and is ready to be used/accessed.
    /// </remarks>
    /// <returns>
    ///     <c>true</c> if this guild is currently connected and ready to be used; otherwise <see langword="false"/>.
    /// </returns>
    bool Available { get; }
    
    /// <summary>
    ///     Gets the built-in role containing all users in this guild.
    /// </summary>
    /// <returns>
    ///     A role object that represents an <c>@everyone</c> role in this guild.
    /// </returns>
    IRole EveryoneRole { get; }

    /// <summary>
    ///     Gets a role in this guild.
    /// </summary>
    /// <param name="id">The identifier for the role.</param>
    /// <returns>
    ///     A role that is associated with the specified <paramref name="id"/>; <see langword="null" /> if none is found.
    /// </returns>
    IRole GetRole(uint id);

    #endregion

    #region Guilds

    /// <summary>
    ///     Leaves this guild.
    /// </summary>
    /// <remarks>
    ///     This method will make the currently logged-in user leave the guild.
    ///     <note>
    ///         If the user is the owner of this guild, use <see cref="IDeletable.DeleteAsync"/> instead.
    ///     </note>
    /// </remarks>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous leave operation.
    /// </returns>
    Task LeaveAsync(RequestOptions options = null);

    #endregion

    #region Guild Bans

    /// <summary>
    ///     Gets a collection of all users banned in this guild.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a read-only collection of
    ///     ban objects that this guild currently possesses, with each object containing the user banned and reason
    ///     behind the ban.
    /// </returns>
    Task<IReadOnlyCollection<IBan>> GetBansAsync(RequestOptions options = null);
    /// <summary>
    ///     Gets a ban object for a banned user.
    /// </summary>
    /// <param name="user">The banned user.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a ban object, which
    ///     contains the user information and the reason for the ban; <see langword="null" /> if the ban entry cannot be found.
    /// </returns>
    Task<IBan> GetBanAsync(IUser user, RequestOptions options = null);
    /// <summary>
    ///     Gets a ban object for a banned user.
    /// </summary>
    /// <param name="userId">The snowflake identifier for the banned user.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a ban object, which
    ///     contains the user information and the reason for the ban; <see langword="null" /> if the ban entry cannot be found.
    /// </returns>
    Task<IBan> GetBanAsync(ulong userId, RequestOptions options = null);
    /// <summary>
    ///     Bans the user from this guild and optionally prunes their recent messages.
    /// </summary>
    /// <param name="user">The user to ban.</param>
    /// <param name="pruneDays">The number of days to remove messages from this user for, and this number must be between [0, 7].</param>
    /// <param name="reason">The reason of the ban to be written in the audit log.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <exception cref="ArgumentException"><paramref name="pruneDays"/> is not between 0 to 7.</exception>
    /// <returns>
    ///     A task that represents the asynchronous add operation for the ban.
    /// </returns>
    Task AddBanAsync(IUser user, int pruneDays = 0, string reason = null, RequestOptions options = null);
    /// <summary>
    ///     Bans the user from this guild and optionally prunes their recent messages.
    /// </summary>
    /// <param name="userId">The snowflake ID of the user to ban.</param>
    /// <param name="pruneDays">The number of days to remove messages from this user for, and this number must be between [0, 7].</param>
    /// <param name="reason">The reason of the ban to be written in the audit log.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <exception cref="ArgumentException"><paramref name="pruneDays"/> is not between 0 to 7.</exception>
    /// <returns>
    ///     A task that represents the asynchronous add operation for the ban.
    /// </returns>
    Task AddBanAsync(ulong userId, int pruneDays = 0, string reason = null, RequestOptions options = null);
    /// <summary>
    ///     Unbans the user if they are currently banned.
    /// </summary>
    /// <param name="user">The user to be unbanned.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous removal operation for the ban.
    /// </returns>
    Task RemoveBanAsync(IUser user, RequestOptions options = null);
    /// <summary>
    ///     Unbans the user if they are currently banned.
    /// </summary>
    /// <param name="userId">The snowflake identifier of the user to be unbanned.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous removal operation for the ban.
    /// </returns>
    Task RemoveBanAsync(ulong userId, RequestOptions options = null);

    #endregion
    
    #region Channels
    
    /// <summary>
    ///     Gets a collection of all channels in this guild.
    /// </summary>
    /// <param name="mode">The <see cref="CacheMode" /> that determines whether the object should be fetched from cache.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a read-only collection of
    ///     generic channels found within this guild.
    /// </returns>
    Task<IReadOnlyCollection<IGuildChannel>> GetChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
    /// <summary>
    ///     Gets a channel in this guild.
    /// </summary>
    /// <param name="id">The identifier for the channel.</param>
    /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the generic channel
    ///     associated with the specified <paramref name="id"/>; <see langword="null" /> if none is found.
    /// </returns>
    Task<IGuildChannel> GetChannelAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
    /// <summary>
    ///     Gets a collection of all text channels in this guild.
    /// </summary>
    /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a read-only collection of
    ///     message channels found within this guild.
    /// </returns>
    Task<IReadOnlyCollection<ITextChannel>> GetTextChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
    /// <summary>
    ///     Gets a text channel in this guild.
    /// </summary>
    /// <param name="id">The snowflake identifier for the text channel.</param>
    /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the text channel
    ///     associated with the specified <paramref name="id"/>; <see langword="null" /> if none is found.
    /// </returns>
    Task<ITextChannel> GetTextChannelAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
    /// <summary>
    ///     Gets a collection of all voice channels in this guild.
    /// </summary>
    /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a read-only collection of
    ///     voice channels found within this guild.
    /// </returns>
    Task<IReadOnlyCollection<IVoiceChannel>> GetVoiceChannelsAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
    /// <summary>
    ///     Gets a voice channel in this guild.
    /// </summary>
    /// <param name="id">The snowflake identifier for the voice channel.</param>
    /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the voice channel associated
    ///     with the specified <paramref name="id"/>; <see langword="null" /> if none is found.
    /// </returns>
    Task<IVoiceChannel> GetVoiceChannelAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
    /// <summary>
    ///     Gets a collection of all category channels in this guild.
    /// </summary>
    /// <param name="mode">The <see cref="CacheMode" /> that determines whether the object should be fetched from cache.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a read-only collection of
    ///     category channels found within this guild.
    /// </returns>
    Task<IReadOnlyCollection<ICategoryChannel>> GetCategoriesAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
    
    // TODO: DefaultChannel
    
    /// <summary>
    ///     Creates a new text channel in this guild.
    /// </summary>
    /// <param name="name">The new name for the text channel.</param>
    /// <param name="func">The delegate containing the properties to be applied to the channel upon its creation.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous creation operation. The task result contains the newly created
    ///     text channel.
    /// </returns>
    Task<ITextChannel> CreateTextChannelAsync(string name, Action<TextChannelProperties> func = null, RequestOptions options = null);
    /// <summary>
    ///     Creates a new voice channel in this guild.
    /// </summary>
    /// <param name="name">The new name for the voice channel.</param>
    /// <param name="func">The delegate containing the properties to be applied to the channel upon its creation.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous creation operation. The task result contains the newly created
    ///     voice channel.
    /// </returns>
    Task<IVoiceChannel> CreateVoiceChannelAsync(string name, Action<VoiceChannelProperties> func = null, RequestOptions options = null);

    #endregion

    #region Invites

    // TODO: Implement Invites

    #endregion

    #region Roles

    // TODO: Implement Roles

    #endregion

    #region Users

    /// <summary>
    ///     Gets a user from this guild.
    /// </summary>
    /// <remarks>
    ///     This method retrieves a user found within this guild.
    ///     <note>
    ///         This may return <see langword="null" /> in the WebSocket implementation due to incomplete user collection in
    ///         large guilds.
    ///     </note>
    /// </remarks>
    /// <param name="id">The identifier of the user.</param>
    /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the guild user
    ///     associated with the specified <paramref name="id"/>; <see langword="null" /> if none is found.
    /// </returns>
    Task<IGuildUser> GetUserAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);

    // TODO: SearchUsersAsync & GetCurrentUserAsync & GetOwnerAsync & DownloadUsersAsync
    
    #endregion

    #region Emotes

    /// <summary>
    ///     Gets a collection of emotes from this guild.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a read-only collection
    ///     of emotes found within the guild.
    /// </returns>
    Task<IReadOnlyCollection<GuildEmote>> GetEmotesAsync(RequestOptions options = null);
    /// <summary>
    ///     Gets a specific emote from this guild.
    /// </summary>
    /// <param name="id">The snowflake identifier for the guild emote.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the emote found with the
    ///     specified <paramref name="id"/>; <see langword="null" /> if none is found.
    /// </returns>
    Task<GuildEmote> GetEmoteAsync(string id, RequestOptions options = null);
    /// <summary>
    ///     Creates a new <see cref="GuildEmote"/> in this guild.
    /// </summary>
    /// <param name="name">The name of the guild emote.</param>
    /// <param name="image">The image of the new emote.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous creation operation. The task result contains the created emote.
    /// </returns>
    Task<GuildEmote> CreateEmoteAsync(string name, Image image, RequestOptions options = null);

    /// <summary>
    ///     Modifies an existing <see cref="GuildEmote"/> in this guild.
    /// </summary>
    /// <param name="emote">The emote to be modified.</param>
    /// <param name="func">The delegate containing the properties to modify the emote with.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous modification operation. The task result contains the modified
    ///     emote.
    /// </returns>
    Task ModifyEmoteNameAsync(GuildEmote emote, Action<string> func, RequestOptions options = null);

    /// <summary>
    ///     Deletes an existing <see cref="GuildEmote"/> from this guild.
    /// </summary>
    /// <param name="emote">The emote to delete.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous removal operation.
    /// </returns>
    Task DeleteEmoteAsync(GuildEmote emote, RequestOptions options = null);

    #endregion

    #region Voice

    /// <summary>
    ///     Gets the users who are muted or deafened in this guild.
    /// </summary>
    /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains
    ///     the collection of muted or deafened users in this guild.
    /// </returns>
    Task<(IReadOnlyCollection<ulong> Muted, IReadOnlyCollection<ulong> Deafened)> GetGuildMuteDeafListAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);

    #endregion
}