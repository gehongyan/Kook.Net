using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using KaiHeiLa.API;
using Model = KaiHeiLa.API.Guild;

namespace KaiHeiLa.Rest;

/// <summary>
///     Represents a REST-based guild/server.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class RestGuild : RestEntity<ulong>, IGuild, IUpdateable
{
    #region RestGuild

    private ImmutableDictionary<uint, RestRole> _roles;
    private ImmutableDictionary<ulong, RestChannel> _channels;
    private ImmutableArray<GuildEmote> _emotes;
    
    /// <inheritdoc />
    public string Name { get; private set; }
    /// <inheritdoc />
    public string Topic { get; private set; }
    /// <inheritdoc />
    public ulong OwnerId { get; private set; }
    /// <inheritdoc />
    public string Icon { get; private set; }
    /// <inheritdoc />
    public NotifyType NotifyType { get; private set; }
    /// <inheritdoc />
    public string Region { get; private set; }
    /// <inheritdoc />
    public bool IsOpenEnabled { get; private set; }
    /// <inheritdoc />
    public uint OpenId { get; private set; }
    /// <inheritdoc />
    public ulong DefaultChannelId { get; private set; }
    /// <inheritdoc />
    public ulong WelcomeChannelId { get; private set; }

    internal bool Available { get; private set; }
    
    /// <summary>
    ///     Gets the built-in role containing all users in this guild.
    /// </summary>
    public RestRole EveryoneRole => GetRole(0);
    
    /// <summary>
    ///     Gets a collection of all roles in this guild.
    /// </summary>
    public IReadOnlyCollection<RestRole> Roles => _roles.ToReadOnlyCollection();
    
    internal RestGuild(BaseKaiHeiLaClient client, ulong id)
        : base(client, id)
    {
    }
    internal static RestGuild Create(BaseKaiHeiLaClient kaiHeiLa, Model model)
    {
        var entity = new RestGuild(kaiHeiLa, model.Id);
        entity.Update(model);
        return entity;
    }

    internal void Update(Model model)
    {
        Name = model.Name;
        Topic = model.Topic;
        OwnerId = model.OwnerId;
        Icon = model.Icon;
        NotifyType = model.NotifyType;
        Region = model.Region;
        IsOpenEnabled = model.EnableOpen;
        OpenId = model.OpenId;
        DefaultChannelId = model.DefaultChannelId;
        WelcomeChannelId = model.WelcomeChannelId;

        Available = true;

        var roles = ImmutableDictionary.CreateBuilder<uint, RestRole>();
        if (model.Roles != null)
        {
            for (int i = 0; i < model.Roles.Length; i++)
                roles[model.Roles[i].Id] = RestRole.Create(KaiHeiLa, this, model.Roles[i]);
        }
        _roles = roles.ToImmutable();
        
        var channels = ImmutableDictionary.CreateBuilder<ulong, RestChannel>();
        if (model.Channels != null)
        {
            for (int i = 0; i < model.Channels.Length; i++)
                channels[model.Channels[i].Id] = RestChannel.Create(KaiHeiLa, model.Channels[i], this);
        }
        _channels = channels.ToImmutable();
    }
    
    #endregion

    #region Generals

    /// <inheritdoc />
    public async Task UpdateAsync(RequestOptions options = null)
        => Update(await KaiHeiLa.ApiClient.GetGuildAsync(Id, options).ConfigureAwait(false));

    /// <inheritdoc />
    public Task LeaveAsync(RequestOptions options = null)
        => GuildHelper.LeaveAsync(this, KaiHeiLa, options);
    #endregion
    
    #region Bans
    /// <summary>
    ///     Gets a collection of all users banned in this guild.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a read-only collection of
    ///     ban objects that this guild currently possesses, with each object containing the user banned and reason
    ///     behind the ban.
    /// </returns>
    public Task<IReadOnlyCollection<RestBan>> GetBansAsync(RequestOptions options = null)
        => GuildHelper.GetBansAsync(this, KaiHeiLa, options);
    /// <summary>
    ///     Gets a ban object for a banned user.
    /// </summary>
    /// <param name="user">The banned user.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a ban object, which
    ///     contains the user information and the reason for the ban; <see langword="null"/> if the ban entry cannot be found.
    /// </returns>
    public Task<RestBan> GetBanAsync(IUser user, RequestOptions options = null)
        => GuildHelper.GetBanAsync(this, KaiHeiLa, user.Id, options);
    /// <summary>
    ///     Gets a ban object for a banned user.
    /// </summary>
    /// <param name="userId">The identifier for the banned user.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a ban object, which
    ///     contains the user information and the reason for the ban; <see langword="null"/> if the ban entry cannot be found.
    /// </returns>
    public Task<RestBan> GetBanAsync(ulong userId, RequestOptions options = null)
        => GuildHelper.GetBanAsync(this, KaiHeiLa, userId, options);

    /// <inheritdoc />
    public Task AddBanAsync(IUser user, int pruneDays = 0, string reason = null, RequestOptions options = null)
        => GuildHelper.AddBanAsync(this, KaiHeiLa, user.Id, pruneDays, reason, options);
    /// <inheritdoc />
    public Task AddBanAsync(ulong userId, int pruneDays = 0, string reason = null, RequestOptions options = null)
        => GuildHelper.AddBanAsync(this, KaiHeiLa, userId, pruneDays, reason, options);

    /// <inheritdoc />
    public Task RemoveBanAsync(IUser user, RequestOptions options = null)
        => GuildHelper.RemoveBanAsync(this, KaiHeiLa, user.Id, options);
    /// <inheritdoc />
    public Task RemoveBanAsync(ulong userId, RequestOptions options = null)
        => GuildHelper.RemoveBanAsync(this, KaiHeiLa, userId, options);
    #endregion

    #region Roles

    /// <summary>
    ///     Gets a role in this guild.
    /// </summary>
    /// <param name="id">The identifier for the role.</param>
    /// <returns>
    ///     A role that is associated with the specified <paramref name="id"/>; <see langword="null"/> if none is found.
    /// </returns>
    public RestRole GetRole(uint id)
    {
        if (_roles.TryGetValue(id, out RestRole value))
            return value;
        return null;
    }

    #endregion

    #region Users
    
    /// <summary>
    ///     Gets a user from this guild.
    /// </summary>
    /// <remarks>
    ///     This method retrieves a user found within this guild.
    /// </remarks>
    /// <param name="id">The identifier of the user.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the guild user
    ///     associated with the specified <paramref name="id"/>; <see langword="null"/> if none is found.
    /// </returns>
    public Task<RestGuildUser> GetUserAsync(ulong id, RequestOptions options = null)
        => GuildHelper.GetUserAsync(this, KaiHeiLa, id, options);

    /// <summary>
    ///     Gets the users who are muted or deafened in this guild.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains
    ///     the collection of muted or deafened users in this guild.
    /// </returns>
    public Task<(IReadOnlyCollection<ulong> Muted, IReadOnlyCollection<ulong> Deafened)> GetGuildMuteDeafListAsync(RequestOptions options = null) 
        => GuildHelper.GetGuildMuteDeafListAsync(this, KaiHeiLa, options);
    
    #endregion
    
    #region Channels
    
    /// <summary>
    ///     Gets a collection of all channels in this guild.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a read-only collection of
    ///     generic channels found within this guild.
    /// </returns>
    public Task<IReadOnlyCollection<RestGuildChannel>> GetChannelsAsync(RequestOptions options = null)
        => GuildHelper.GetChannelsAsync(this, KaiHeiLa, options);
    /// <summary>
    ///     Gets a channel in this guild.
    /// </summary>
    /// <param name="id">The identifier for the channel.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the generic channel
    ///     associated with the specified <paramref name="id"/>; <see langword="null"/> if none is found.
    /// </returns>
    public Task<RestGuildChannel> GetChannelAsync(ulong id, RequestOptions options = null)
        => GuildHelper.GetChannelAsync(this, KaiHeiLa, id, options);
    /// <summary>
    ///     Gets a text channel in this guild.
    /// </summary>
    /// <param name="id">The snowflake identifier for the text channel.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the text channel
    ///     associated with the specified <paramref name="id"/>; <see langword="null"/> if none is found.
    /// </returns>
    public async Task<RestTextChannel> GetTextChannelAsync(ulong id, RequestOptions options = null)
    {
        var channel = await GuildHelper.GetChannelAsync(this, KaiHeiLa, id, options).ConfigureAwait(false);
        return channel as RestTextChannel;
    }
    /// <summary>
    ///     Gets a collection of all text channels in this guild.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a read-only collection of
    ///     message channels found within this guild.
    /// </returns>
    public async Task<IReadOnlyCollection<RestTextChannel>> GetTextChannelsAsync(RequestOptions options = null)
    {
        var channels = await GuildHelper.GetChannelsAsync(this, KaiHeiLa, options).ConfigureAwait(false);
        return channels.OfType<RestTextChannel>().ToImmutableArray();
    }
    /// <summary>
    ///     Gets a voice channel in this guild.
    /// </summary>
    /// <param name="id">The snowflake identifier for the voice channel.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the voice channel associated
    ///     with the specified <paramref name="id"/>; <see langword="null"/> if none is found.
    /// </returns>
    public async Task<RestVoiceChannel> GetVoiceChannelAsync(ulong id, RequestOptions options = null)
    {
        var channel = await GuildHelper.GetChannelAsync(this, KaiHeiLa, id, options).ConfigureAwait(false);
        return channel as RestVoiceChannel;
    }
    /// <summary>
    ///     Gets a collection of all voice channels in this guild.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a read-only collection of
    ///     voice channels found within this guild.
    /// </returns>
    public async Task<IReadOnlyCollection<RestVoiceChannel>> GetVoiceChannelsAsync(RequestOptions options = null)
    {
        var channels = await GuildHelper.GetChannelsAsync(this, KaiHeiLa, options).ConfigureAwait(false);
        return channels.OfType<RestVoiceChannel>().ToImmutableArray();
    }
    /// <summary>
    ///     Gets a collection of all category channels in this guild.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a read-only collection of
    ///     category channels found within this guild.
    /// </returns>
    public async Task<IReadOnlyCollection<RestCategoryChannel>> GetCategoryChannelsAsync(RequestOptions options = null)
    {
        var channels = await GuildHelper.GetChannelsAsync(this, KaiHeiLa, options).ConfigureAwait(false);
        return channels.OfType<RestCategoryChannel>().ToImmutableArray();
    }
    
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
    public Task<RestTextChannel> CreateTextChannelAsync(string name, Action<TextChannelProperties> func = null, RequestOptions options = null)
        => GuildHelper.CreateTextChannelAsync(this, KaiHeiLa, name, options, func);
    /// <summary>
    ///     Creates a voice channel with the provided name.
    /// </summary>
    /// <param name="name">The name of the new channel.</param>
    /// <param name="func">The delegate containing the properties to be applied to the channel upon its creation.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <exception cref="ArgumentNullException"><paramref name="name" /> is <see langword="null"/>.</exception>
    /// <returns>
    ///     The created voice channel.
    /// </returns>
    public Task<RestVoiceChannel> CreateVoiceChannelAsync(string name, Action<VoiceChannelProperties> func = null, RequestOptions options = null)
        => GuildHelper.CreateVoiceChannelAsync(this, KaiHeiLa, name, options, func);
    
    #endregion
    
    #region Emotes
    
    /// <inheritdoc />
    public Task<IReadOnlyCollection<GuildEmote>> GetEmotesAsync(RequestOptions options = null)
        => GuildHelper.GetEmotesAsync(this, KaiHeiLa, options);
    /// <inheritdoc />
    public Task<GuildEmote> GetEmoteAsync(string id, RequestOptions options = null)
        => GuildHelper.GetEmoteAsync(this, KaiHeiLa, id, options);
    /// <inheritdoc />
    public Task<GuildEmote> CreateEmoteAsync(string name, Image image, RequestOptions options = null)
        => GuildHelper.CreateEmoteAsync(this, KaiHeiLa, name, image, options);
    /// <inheritdoc />
    /// <exception cref="ArgumentNullException"><paramref name="func"/> is <see langword="null"/>.</exception>
    public Task ModifyEmoteNameAsync(GuildEmote emote, Action<string> func, RequestOptions options = null)
        => GuildHelper.ModifyEmoteNameAsync(this, KaiHeiLa, emote, func, options);
    /// <inheritdoc />
    public Task DeleteEmoteAsync(GuildEmote emote, RequestOptions options = null)
        => GuildHelper.DeleteEmoteAsync(this, KaiHeiLa, emote.Id, options);
    
    #endregion
    
    #region IGuild
    /// <inheritdoc />
    bool IGuild.Available => Available;
    
    /// <inheritdoc />
    IRole IGuild.GetRole(uint id)
        => GetRole(id);
    
    /// <inheritdoc />
    IRole IGuild.EveryoneRole => EveryoneRole;

    /// <inheritdoc />
    async Task<IGuildUser> IGuild.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
    {
        if (mode == CacheMode.AllowDownload)
            return await GetUserAsync(id, options).ConfigureAwait(false);
        else
            return null;
    }
    
    /// <inheritdoc />
    async Task<IReadOnlyCollection<IBan>> IGuild.GetBansAsync(RequestOptions options)
        => await GetBansAsync(options).ConfigureAwait(false);
    /// <inheritdoc/>
    async Task<IBan> IGuild.GetBanAsync(IUser user, RequestOptions options)
        => await GetBanAsync(user, options).ConfigureAwait(false);
    /// <inheritdoc/>
    async Task<IBan> IGuild.GetBanAsync(ulong userId, RequestOptions options)
        => await GetBanAsync(userId, options).ConfigureAwait(false);
    
    /// <inheritdoc />
    async Task<IReadOnlyCollection<IGuildChannel>> IGuild.GetChannelsAsync(CacheMode mode, RequestOptions options)
    {
        if (mode == CacheMode.AllowDownload)
            return await GetChannelsAsync(options).ConfigureAwait(false);
        else
            return ImmutableArray.Create<IGuildChannel>();
    }
    /// <inheritdoc />
    async Task<IGuildChannel> IGuild.GetChannelAsync(ulong id, CacheMode mode, RequestOptions options)
    {
        if (mode == CacheMode.AllowDownload)
            return await GetChannelAsync(id, options).ConfigureAwait(false);
        else
            return null;
    }
    
    /// <inheritdoc />
    async Task<IReadOnlyCollection<ITextChannel>> IGuild.GetTextChannelsAsync(CacheMode mode, RequestOptions options)
    {
        if (mode == CacheMode.AllowDownload)
            return await GetTextChannelsAsync(options).ConfigureAwait(false);
        else
            return ImmutableArray.Create<ITextChannel>();
    }
    /// <inheritdoc />
    async Task<ITextChannel> IGuild.GetTextChannelAsync(ulong id, CacheMode mode, RequestOptions options)
    {
        if (mode == CacheMode.AllowDownload)
            return await GetTextChannelAsync(id, options).ConfigureAwait(false);
        else
            return null;
    }
    /// <inheritdoc />
    async Task<IReadOnlyCollection<IVoiceChannel>> IGuild.GetVoiceChannelsAsync(CacheMode mode, RequestOptions options)
    {
        if (mode == CacheMode.AllowDownload)
            return await GetVoiceChannelsAsync(options).ConfigureAwait(false);
        else
            return ImmutableArray.Create<IVoiceChannel>();
    }
    /// <inheritdoc />
    async Task<IVoiceChannel> IGuild.GetVoiceChannelAsync(ulong id, CacheMode mode, RequestOptions options)
    {
        if (mode == CacheMode.AllowDownload)
            return await GetVoiceChannelAsync(id, options).ConfigureAwait(false);
        else
            return null;
    }
    /// <inheritdoc />
    async Task<IReadOnlyCollection<ICategoryChannel>> IGuild.GetCategoriesAsync(CacheMode mode, RequestOptions options)
    {
        if (mode == CacheMode.AllowDownload)
            return await GetCategoryChannelsAsync(options).ConfigureAwait(false);
        else
            return null;
    }
    
    /// <inheritdoc />
    async Task<ITextChannel> IGuild.CreateTextChannelAsync(string name, Action<TextChannelProperties> func, RequestOptions options)
        => await CreateTextChannelAsync(name, func, options).ConfigureAwait(false);
    /// <inheritdoc />
    async Task<IVoiceChannel> IGuild.CreateVoiceChannelAsync(string name, Action<VoiceChannelProperties> func, RequestOptions options)
        => await CreateVoiceChannelAsync(name, func, options).ConfigureAwait(false);
    
    /// <inheritdoc />
    async Task<(IReadOnlyCollection<ulong> Muted, IReadOnlyCollection<ulong> Deafened)> IGuild.GetGuildMuteDeafListAsync(CacheMode mode, RequestOptions options)
    {
        if (mode == CacheMode.AllowDownload)
            return await GetGuildMuteDeafListAsync(options).ConfigureAwait(false);
        else
            return (null, null);
    }

    #endregion

    /// <summary>
    ///     Returns the name of the guild.
    /// </summary>
    /// <returns>
    ///     The name of the guild.
    /// </returns>
    public override string ToString() => Name;
    private string DebuggerDisplay => $"{Name} ({Id})";
}